using SIPlus;
using System.Diagnostics;
using System.Collections;

namespace SIPlus.Test
{
    internal class TestFuncValueRetriever : IValueRetriever {
        public object? Retrieve(InvocationContext value) {
            return "test";
        }

        public void Dispose() { }
    }

    internal class TestFunction : IFunction {
        public IValueRetriever Value(IValueRetriever? parent, List<IValueRetriever> parameters) {
            return new TestFuncValueRetriever();
        }

        public void Dispose() { }
    }

    internal class Program {
        private class Inner { public string Text = "World"; }
        private class Outer { public Inner Inner = new(); }

        static void Main(string[] args) {
            var testVal = new Outer();

            Parser parser = new();
            parser.Context().UseSTL();
            parser.Context().AddFunction("test", new TestFunction());

            using var varContext = parser.Context().Builder().Default(testVal).With("val", 12321).Build();

            parser.TestInterpolation("Base", "Hello, { .Text }", testVal.Inner, "Hello, World");
            parser.TestInterpolation("Func", "Hello, { test }", testVal, "Hello, test");
            parser.TestExpression("Base - Expr", ".Inner", testVal, v => v == testVal.Inner);
            parser.TestExpression("Func - Expr", "test", testVal, v => v.Equals("test"));
            parser.TestExpression("Base - Func", "@get_2 => ( 2 ); @get_2", testVal, v => v.Equals(2));
            parser.TestExpression("Base - Variable", "$val", new ParseOpts().AddGlobal("val"), varContext, v => v.Equals(12321));
            parser.TestExpression("Base - Indexer", ".[1]", new int[] { 1, 2, 3 }, v => v.Equals(2));
            parser.TestExpression("Base - Array", "[1, 2, 3]", testVal, v => {
                return v is IEnumerable e ? e.Cast<object>().SequenceEqual([1, 2, 3]) : false;
            });

            parser.Dispose();
            
            //Force GC cleanup to make sure deleters work properly.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public static class ParserExtensions {
        public static void TestInterpolation(
            this Parser parser,
            string name,
            string text,
            object defaultVal,
            string expected
        ) {
            using var context = parser.Context()
                .Builder()
                .Default(defaultVal)
                .Build();

            TestInterpolation(parser, name, text, context, expected);
        }
          
        public static void TestInterpolation(
            this Parser parser, 
            string name,
            string text, 
            InvocationContext context,
            string expected
        ) {
            Console.Write($"Testing {name}");

            var watch = Stopwatch.StartNew();
            using var constructor = parser.GetInterpolation(text);
            var parseTime = watch.Elapsed;

            var value = constructor.Construct(context);
            var exTime = watch.Elapsed;

            var passed = value == expected;
    
            Console.WriteLine($" {parseTime}/{exTime - parseTime} - {(passed ? "PASSED" : "FAILED")}");


            if(!passed) {
                throw new Exception($"Test {name} failed.");
            }
        }

        public static void TestExpression(
            this Parser parser,
            string name,
            string text,
            object defaultVal,
            Func<object?, bool> test
        ) {
            using var context = parser.Context().Builder().Default(defaultVal).Build();

            TestExpression(parser, name, text, new(), context, test);
        }

        public static void TestExpression(
            this Parser parser,
            string name,
            string text,
            ParseOpts parseOpts,
            InvocationContext context,
            Func<object?, bool> test
        ) {
            Console.Write($"Testing {name}");

            var watch = Stopwatch.StartNew();
            using var constructor = parser.GetExpression(text, parseOpts);
            var parseTime = watch.Elapsed;

            var value = constructor.Retrieve(context);
            var exTime = watch.Elapsed;

            var passed = test(value);

            Console.WriteLine($" {parseTime}/{exTime - parseTime} - {(passed ? "PASSED" : "FAILED")}");

            if (!passed) {
                throw new Exception($"Test {name} failed.");
            }
        }
    }
}
