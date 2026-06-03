using SIPlus.Test.Extensions;
using SIPlus.Test.Data;
using SIPlus.Test.Models;
using Xunit;

namespace SIPlus.Test {
    public class TestInterpolations {
        [Fact]
        public void BasicInterpolation() {
            using var parser = Util.GetBaseTestingParser();
            parser.Context().AddFunction("test", new TestFunction());

            var testVal = new Person() {
                FirstName = "John",
                LastName = "Doe"
            };
            
            parser.TestInterpolation("Base", "Hello, { .FirstName }", testVal, "Hello, John");
            parser.TestInterpolation("Func", "Hello, { test }", testVal, "Hello, test");
            parser.TestInterpolation("Base - Converters", "{.}", testVal, "John Doe");
        }
    }
}
