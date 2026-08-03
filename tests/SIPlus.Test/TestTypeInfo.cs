using SIPlus.NET;
using System.Collections;
using Xunit;

namespace SIPlus.Test {
    public class TestTypeInfo {
        [Fact]
        public void TypeInfo() {
            using var parser = Util.GetBaseTestingParser();
            var data = new TestData();

            var expr = parser.GetExpression(".Foo");
            var result = expr.Retrieve(parser.Context().Builder().Default(data).Build());

            Assert.Equal(data.Foo, result);
        }

        [Fact]
        public void TypeInfoIndexer() {
            using var parser = Util.GetBaseTestingParser();
            var data = new TestData();

            var expr = parser.GetExpression(".[\"2\"]");
            var result = expr.Retrieve(parser.Context().Builder().Default(data).Build());

            Assert.Equal("2", result);
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

            public object? Access(object? value, string name) {
                if(value is not TestData data) return null;

                switch (name) {
                    case "Foo":
                        return data.Foo;

                    default:
                        throw new Exception($"No suitable property '{name}'");
                }
            }

            object? ITypeInfoIndexer.Index(ParserContext context, object? list, object? index) {
                return index;
            }

            public bool IsIterable(object? value) => false;
            public IEnumerator Iterate(object? value) => throw new NotImplementedException();
            public void Dispose() { }
        }
    }
}
