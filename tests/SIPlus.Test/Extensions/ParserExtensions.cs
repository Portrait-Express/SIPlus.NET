using SIPlus.NET;
using System.Diagnostics;

namespace SIPlus.Test.Extensions
{
    public static class ParserExtensions {
        public static void TestInterpolation(
            this Parser parser,
            string name,
            string text,
            object? defaultVal,
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

            Console.WriteLine($" {parseTime}/{exTime - parseTime} - {(value == expected ? "PASSED" : "FAILED")}");
            Assert.Equal(expected, value);
        }

        public static void TestExpression(
            this Parser parser,
            string name,
            string text,
            object? defaultVal,
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

            Assert.True(passed, $"Expression \"{text}\" did not pass");
        }
    }
}
