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
            using var varContext = _parser.Context().Builder().Default(new(testVal)).With("val", new(12321)).Build();

            //Make sure default type mappings work
            _parser.TestExpression("TypeMap - Null", ". | type", null, v => Assert.Equal("null", v.NetValue));
            _parser.TestExpression("TypeMap - Number", ". | type", 2, v => Assert.Equal("long", v.NetValue));
            _parser.TestExpression("TypeMap - String", ". | type", "text", v => Assert.Equal("string", v.NetValue));
            _parser.TestExpression("TypeMap - Bool", ". | type", false, v => Assert.Equal("boolean", v.NetValue));

            //Test functionality
            _parser.TestExpression("Func - Expr", "test", testVal, v => Assert.Equal("test", v.NetValue));
            _parser.TestExpression("Base - Func", "@get_2 => ( 2 ); @get_2", testVal, v => Assert.Equal(2L, v.NetValue));
            _parser.TestExpression("Base - Variable", "$val", new ParseOpts().AddGlobal("val"), varContext, v => Assert.Equal(12321L, v.NetValue));
            _parser.TestExpression("Base - Indexer", ".[1]", new int[] { 1, 2, 3 }, v => Assert.Equal(2L, v.NetValue));
            _parser.TestExpression("Base - Array", "[1, 2, 3]", testVal, v => {
                Assert.True(
                    v.NetValue is IEnumerable e ? e.Cast<object>().SequenceEqual([1L, 2L, 3L]) : false,
                    "Array test failed.");
            });
        }

        [Fact]
        public void SetExpression() {
            using var expr = _parser.GetExpression("const var $s = set_new; set_add $s 1; set_add $s 2; set_add $s 2; $s | map .");
            var result = expr.Retrieve(_parser.Context().Builder().Build());

            var resultArr = result.AsArray().Select(v => v.AsInt())
                .OrderBy(v => v);

            Assert.Equal([1, 2], resultArr.ToArray());
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
