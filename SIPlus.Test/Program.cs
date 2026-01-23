using SIPlus.Text;
using System.Diagnostics;

namespace SIPlus.Test
{
    internal class TestFuncValueRetriever : IValueRetriever {
        public object Retrieve(InvocationContext context) {
            return "test";
        }
    }

    internal class TestFunction : IFunction {
        public IValueRetriever Value(IValueRetriever parent, IEnumerable<IValueRetriever> parameters) {
            return new TestFuncValueRetriever();
        }
    }

    internal class Program {
        static void Main(string[] args) {
            SIPlusParser parser = new();
            parser.Context().AddFunction("test", new TestFunction());

            parser.TestInterpolation("Base", "Hello, { .test }", new { test = "World" }, "Hello, World");
            parser.TestInterpolation("Func", "Hello, { test }", new { }, "Hello, test");
        }
    }

    public static class ParserExtensions {
        public static void TestInterpolation(
            this SIPlusParser parser,
            string name,
            string text,
            object defaultVal,
            string expected
        ) {
            var context = parser.Context().Builder().UseDefault(defaultVal).Build();

            TestInterpolation(parser, name, text, context, expected);
        }
          
        public static void TestInterpolation(
            this SIPlusParser parser, 
            string name,
            string text, 
            InvocationContext context,
            string expected
        ) {
            Console.Write($"Testing {name}");

            var watch = Stopwatch.StartNew();
            var constructor = parser.GetInterpolation(text);
            var parseTime = watch.Elapsed;

            var value = constructor.Construct(context);
            var exTime = watch.Elapsed;

            var passed = value == expected;

            Console.WriteLine($" {parseTime}/{exTime - parseTime} - {(passed ? "PASSED" : "FAILED")}");

            if(!passed) {
                throw new Exception($"Test {name} failed.");
            }
        }
    }
}
