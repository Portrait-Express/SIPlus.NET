using SIPlus.NET.Internal;
using SIPlus.NET.Internal.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SIPlus.NET {
    public class SIValue : IEnumerable<SIValue>, IDisposable {
        private object? _netValue;
        private ITypeInfo? _netType;
        private SIPlusNative.DataContainerHandle? _nativeHandle;

        [MemberNotNullWhen(true, nameof(_nativeHandle))]
        [MemberNotNullWhen(false, nameof(_netValue), nameof(_netType))]
        private bool IsNative {
            get => _nativeHandle != null;
        }

        /// <summary>
        /// Returns a value to be passed to an ITypeInfo function.
        /// </summary>
        /// <remarks>
        /// Do not use this for logic in your code, the return value will 
        /// not be translated to NET types for primitives. Use the As* and 
        /// Is* member functions, or <see cref="NetValue"/> for that.
        /// </remarks>
        public object? Value { 
            get {
                if(IsNative) {
                    return _nativeHandle;
                } else {
                    return _netValue;
                }
            }
        }

        public ITypeInfo Type {
            get {
                if(IsNative) {
                    Util.AssertSuccess(SIPlusNative.siplus_data_type(out var type, _nativeHandle));
                    return new NativeTypeInfo(type);
                } else {
                    return _netType;
                }
            }
        }

        /// <summary>
        /// The value, cast to a valid .NET type
        /// </summary>
        public object? NetValue {
            get {
                if (IsNative) {
                    if (IsInt()) return AsInt();
                    if (IsFloat()) return AsFloat();
                    if (IsString()) return AsString();
                    if (IsBool()) return AsBool();
                    if (IsArray()) return AsArray().Select(v => v.NetValue).ToArray();
                    if (IsNull()) return null;
                    throw new InvalidOperationException($"Cannot convert '{Type.Name}' to a valid .NET type");
                } else {
                    return _netValue;
                }
            }
        }

        public bool IsIterable => Type.IsIterable(Value);


        internal SIValue(SIPlusNative.DataContainerHandle handle) {
            Util.AssertSuccess(SIPlusNative.siplus_data_type(out var type, handle));
            var data = SIPlusNative.siplus_type_data_ptr(type);

            if(data != IntPtr.Zero) {
                Util.AssertSuccess(SIPlusNative.siplus_data_ptr(out var ptr, handle));

                _netType = (ITypeInfo)GCHandle.FromIntPtr(data).Target!;
                _netValue = GCHandle.FromIntPtr(ptr).Target;
            } else {
                _nativeHandle = handle;
            }
        }

        public SIValue(object? value, ITypeInfo type) {
            _netValue = value;
            _netType = type;
        }

        public SIValue(object? value) {
            _nativeHandle = Util.TryToNativeType(value);

            if(_nativeHandle == null) {
                _netValue = value;

                var attr = value!.GetType().GetCustomAttribute<SIPlusTypeAttribute>();
                if (attr != null) {
                    if (!attr.TypeInfoType.IsAssignableTo(typeof(ITypeInfo))) {
                        throw new SIPlusException(
                            $"Type '{attr.TypeInfoType}' from SIPlusType attribute " +
                            $"on type '{value.GetType()}' does not inherit from ITypeInfo."
                        );
                    }

                     _netType = (ITypeInfo?)Activator.CreateInstance(attr.TypeInfoType) ??
                        throw new InvalidOperationException("ITypeInfo used in SIPlusType attribute must have a parameterless constructor");
                } else {
                    _netType = new NETTypeInfo();
                }
            }
        }

        public SIValue Access(string property) {
            return Type.Access(Value, property);
        }

        public IEnumerator<SIValue> Iterate() {
            return Type.Iterate(Value);
        }

        internal unsafe SIPlusNative.DataContainerHandle ToNative() {
            if(IsNative) {
                return _nativeHandle;
            } else {
                var handle = GCHandle.Alloc(Value);
                GlobalStaticStorage.Store(handle);

                return SIPlusNative.siplus_data_make(
                    _netType.GetNativeTypeInfo().Handle, GCHandle.ToIntPtr(handle), &ObjectDataDeleter);
            }
        }

        public bool IsInt() => IsNative && SIPlusNative.siplus_data_is_int(_nativeHandle);
        public bool IsFloat() => IsNative && SIPlusNative.siplus_data_is_float(_nativeHandle);
        public bool IsNull() => IsNative && SIPlusNative.siplus_data_is_null(_nativeHandle);
        public bool IsString() => IsNative && SIPlusNative.siplus_data_is_string(_nativeHandle);
        public bool IsArray() => IsNative && SIPlusNative.siplus_data_is_array(_nativeHandle);
        public bool IsBool() => IsNative && SIPlusNative.siplus_data_is_bool(_nativeHandle);

        public long AsInt() {
            if(IsInt()) {
                Util.AssertSuccess(SIPlusNative.siplus_data_as_int(out var result, _nativeHandle));
                return result;
            } else {
                throw new InvalidOperationException("Value is not an int");
            }
        }

        public double AsFloat() {
            if (IsFloat()) {
                Util.AssertSuccess(SIPlusNative.siplus_data_as_float(out var result, _nativeHandle));
                return result;
            } else {
                throw new InvalidOperationException("Value is not a float");
            }
        }

        public bool AsBool() {
            if (IsNative && IsBool()) {
                Util.AssertSuccess(SIPlusNative.siplus_data_as_bool(out var result, _nativeHandle));
                return result == 0 ? false : true;
            } else {
                throw new InvalidOperationException("Value is not a bool");
            }
        }

        public object? AsNull() {
            if(IsNull()) {
                return null;
            } else {
                throw new InvalidOperationException("Value is not an int");
            }
        }

        public string AsString() {
            if(IsString()) {
                Util.AssertSuccess(SIPlusNative.siplus_data_as_string(out var result, _nativeHandle));
                return result.Value ?? "";
            } else {
                throw new InvalidOperationException("Value is not an int");
            }
        }

        public SIValue[] AsArray() {
            if(IsArray()) {
                return [.. this];
            } else {
                throw new InvalidOperationException("Value is not an array");
            }
        }


        IEnumerator<SIValue> IEnumerable<SIValue>.GetEnumerator() {
            return Iterate();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return Iterate();
        }


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(disposing) {
                _netType?.Dispose();
                if(_netValue is IDisposable d) d.Dispose();
                _nativeHandle?.Dispose();
            }
        }

        ~SIValue() {
            Dispose(false);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void ObjectDataDeleter(nint data) {
            var handle = GCHandle.FromIntPtr(data);
            handle.Free();
            GlobalStaticStorage.Release(handle);
        }
    }
}
