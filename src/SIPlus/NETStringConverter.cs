using SIPlus.NET.Internal;

namespace SIPlus.NET {
    public class NETStringConverter : IDataConverter {
        public bool CanConvert(ITypeInfo from, ITypeInfo to) {
            return from.Name == new NETTypeInfo().Name && to.Name == "string";
        }

        public SIValue Convert(SIValue from, ITypeInfo to) {
            if (to.Name != "string") {
                throw new Exception($"Cannot convert to type {to.Name}");
            }

            return new(from.NetValue?.ToString() ?? "null");
        }

        public void Dispose() { }
    }
}
