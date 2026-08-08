using SIPlus.NET;
using System.Collections;
using Xunit;

namespace SIPlus.Test {
    public class TestTypeInfo : IDisposable {
        private Parser _parser = Util.GetBaseTestingParser();

        public void Dispose() {
            _parser.Dispose();
        }

        [Fact]
        public void TypeInfo() {
            var data = new TestData();

            var expr = _parser.GetExpression(".Bar");
            var result = expr.Retrieve(_parser.Context().Builder().Default(new(data)).Build());

            object? expected = 2L;
            object? actual = result.NetValue;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestIdentityTypeInfo() {
            var data = new TestData();

            var expr = _parser.GetExpression(".");
            var result = expr.Retrieve(_parser.Context().Builder().Default(new(data)).Build());

            Assert.Equal(data, result.NetValue);
        }

        [Fact]
        public void TypeInfoIndexer() {
            using var parser = Util.GetBaseTestingParser();
            var data = new TestData();

            var expr = parser.GetExpression(".[\"2\"]");
            var result = expr.Retrieve(parser.Context().Builder().Default(new(new TestData())).Build());

            Assert.Equal("2", result.NetValue);
        }

        [SIPlusType(typeof(TestingType))]
        class TestData {
            public class Inner {
                public int Value = 2;
            }

            public Inner Foo { get; set; } = new();
        }

        internal class TestingType : ITypeInfo, ITypeInfoIndexer {
            public TestingType() { }
            public string Name => "TestType";

            public SIValue Access(object? value, string name) {
                if(value is not TestData data) return null;

                switch (name) {
                    case "Foo":
                        return new(data.Foo);

                    case "Bar":
                        return new(2);

                    default:
                        throw new Exception($"No suitable property '{name}'");
                }
            }

            SIValue ITypeInfoIndexer.Index(ParserContext context, object? list, SIValue index) {
                return index;
            }

            public bool IsIterable(object? value) => false;
            public IEnumerator<SIValue> Iterate(object? value) => throw new NotImplementedException();
            public void Dispose() { }
        }
    }

}
