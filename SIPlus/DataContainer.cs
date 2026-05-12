using System.Collections;

namespace SIPlus.NET {
    public class DataContainer {
        public object? Value;
        public ITypeInfo Type;
        public bool IsIterable => Type.IsIterable(Value);

        public DataContainer(object? value, ITypeInfo type) {
            Value = value;
            Type = type;
        }

        public object? Access(string property) {
            return Type.Access(Value, property);
        }

        public IEnumerator Iterate() {
            return Type.Iterate(Value);
        }
    }
}
