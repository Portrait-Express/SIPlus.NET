using CSIPlus.Internal.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal {
    internal class NETTypeInfo : ITypeInfo {
        internal static readonly NativeTypeInfo NativeInstance = new NETTypeInfo().GetNativeTypeInfo();

        public string Name => "System.Object";

        public object? Access(object? value, string name) {
            if(value == null) {
                throw new Exception($"Cannot access property '{name}' on null");
            }

            var type = value.GetType();

            var member = type.GetField(name);
            if(member != null) {
                return member.GetValue(value);
            }

            var prop = type.GetProperty(name);
            if (prop != null) {
                return prop.GetValue(value);
            }

            throw new Exception($"Object of type '{type.Name}' has no accessible property '{name}'");
        }

        public bool IsIterable(object? value) {
            if (value == null) {
                return false;
            }

            if(value is IEnumerable) {
                return true;
            }

            return false;
        }

        public IEnumerator Iterate(object? value) {
            if (value == null) {
                throw new Exception("null is not iterable");
            }

            if (value is IEnumerable enumerable) {
                return enumerable.GetEnumerator();
            }

            throw new Exception($"{value.GetType().Name} is not iterable");
        }

        public void Dispose() { }
    }
}
