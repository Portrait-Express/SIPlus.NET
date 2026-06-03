using SIPlus.NET;
using System.Collections;
using Xunit;

namespace SIPlus.Test {
    public class TestTypeInfo {
        [Fact]
        public void TypeInfo() {
            using var parser = Util.GetBaseTestingParser();
            var data = new DataContainer(null, new TestingType());

            var expr = parser.GetExpression(".a");
            var result = expr.Retrieve(parser.Context().Builder().Default(data).Build());

            Assert.Equal(123, result);
        }


        internal class TestingType : ITypeInfo {
            public TestingType() { }
            public string Name => "TestType";

            public object? Access(object? value, string name) {
                switch (name) {
                    case "a":
                        return 123;

                    default:
                        throw new Exception($"No suitable property '{name}'");
                }
            }

            public bool IsIterable(object? value) => false;
            public IEnumerator Iterate(object? value) => throw new NotImplementedException();
            public void Dispose() { }
        }
    }
}
