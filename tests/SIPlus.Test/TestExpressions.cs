using Xunit;
using SIPlus.NET;
using SIPlus.Test.Data;
using SIPlus.Test.Extensions;
using SIPlus.Test.Models;
using System.Collections;

namespace SIPlus.Test {
    public class TestExpressions : IDisposable {
        private Parser _parser;

        public TestExpressions() {
            _parser = Util.GetBaseTestingParser();
            _parser.Context().AddFunction("test", new TestFunction());
        }

        [Fact]
        public void BasicExpressions() {
            var testVal = new Person();
            using var varContext = _parser.Context().Builder().Default(testVal).With("val", 12321).Build();

            //Make sure default type mappings work
            _parser.TestExpression("TypeMap - Null", ". | type", null, "null".Equals);
            _parser.TestExpression("TypeMap - Number", ". | type", 2, "long".Equals);
            _parser.TestExpression("TypeMap - String", ". | type", "text", "string".Equals);
            _parser.TestExpression("TypeMap - Bool", ". | type", false, "boolean".Equals);

            //Test functionality
            _parser.TestExpression("Func - Expr", "test", testVal, "test".Equals);
            _parser.TestExpression("Base - Func", "@get_2 => ( 2 ); @get_2", testVal, 2.Equals);
            _parser.TestExpression("Base - Variable", "$val", new ParseOpts().AddGlobal("val"), varContext, 12321.Equals);
            _parser.TestExpression("Base - Indexer", ".[1]", new int[] { 1, 2, 3 }, 2.Equals);
            _parser.TestExpression("Base - Array", "[1, 2, 3]", testVal, v => {
                return v is IEnumerable e ? e.Cast<object>().SequenceEqual([1, 2, 3]) : false;
            });
        }

        [Fact]
        public void SetExpression() {
            using var expr = _parser.GetExpression("const var $s = set_new; set_add $s 1; set_add $s 2; set_add $s 2; $s");
            var result = expr.Retrieve(_parser.Context().Builder().Build());

            Assert.Equal(
                Assert.IsAssignableFrom<IEnumerable>(result).Cast<object>(),
                [1, 2]
            );
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if(disposing) {
                _parser.Dispose();
            }
        }

        ~TestExpressions() {
            Dispose(false);
        }
    }
}
