using SIPlus.NET.Internal;
using SIPlus.NET.Internal.Extensions;
using System.Collections;
using System.Reflection;

namespace SIPlus.NET {
    public class NETTypeInfo : ITypeInfo, ITypeInfoIndexer {
        internal static readonly NativeTypeInfo NativeInstance = new NETTypeInfo().GetNativeTypeInfo();

        public string Name => "System.Object";

        public SIValue Access(object? value, string name) {
            if(value == null) {
                throw new Exception($"Cannot access property '{name}' on null");
            }

            var type = value.GetType();

            MemberInfo? member = type.GetField(name);
            if (member == null) {
                member = type.GetProperty(name);
            }

            if(member is FieldInfo field) {
                return new(field.GetValue(value));
            } else if(member is PropertyInfo prop) {
                return new(prop.GetValue(value));
            } else {
                throw new Exception($"Object of type '{type.Name}' has no accessible property '{name}'");
            }
        }

        public SIValue Index(ParserContext context, object? list, SIValue index) {
            var indexValue = index.NetValue;

            if (index != null && list is IDictionary idictionary && indexValue != null) {
                return new(idictionary[indexValue]);
            }

            if(TryIndex(indexValue, out var idx)) {
                if (list is IList ilist) return new(ilist[idx]);
                if (list is IEnumerable enumerable) return new(enumerable.ElementAt(idx));
            }

            if(indexValue?.GetType() == typeof(string)) {
                return Access(list, (string)indexValue);
            }

            throw new InvocationException(
                $"Cannot use '{indexValue}' to index object of type '{list}'");
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

        public IEnumerator<SIValue> Iterate(object? value) {
            if (value == null) {
                throw new Exception("null is not iterable");
            }

            if (value is IEnumerable enumerable) {
                foreach(var v in enumerable) {
                    yield return v is SIValue s ? s : new SIValue(v);
                }
            }

            throw new Exception($"{value.GetType().Name} is not iterable");
        }

        private static bool TryIndex(object? value, out int result) {
            if (value is ulong ul) {
                if (ul > int.MaxValue) {
                    result = 0;
                    return false;
                }

                result = (int)ul;
                return true;
            }

            if (value is long l) {
                if (l > int.MaxValue || l < int.MinValue) {
                    result = 0;
                    return false;
                }

                result = (int)l;
                return true;
            }

            if (value is uint ui) {
                if (ui > int.MaxValue) {
                    result = 0;
                    return false;
                }

                result = (int)ui; 
                return true; 
            }

            if (value is int i) { result = i; return true; }
            if (value is ushort us) { result = us; return true; }
            if (value is short s) { result = s; return true; }
            if (value is byte ub) { result = ub; return true; }
            if (value is char b) { result = b; return true; }

            result = 0; 
            return false;
        }

        public void Dispose() { }
    }
}
