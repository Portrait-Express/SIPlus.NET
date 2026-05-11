using SIPlus.NET;
using SIPlus.NET.Internal;
using System.Collections;
using System.Runtime.InteropServices;

namespace SIPlus.Internal {
    internal class NativeTypeInfo : ITypeInfo {
        internal SIPlusNative.TypeInfoHandle Handle;

        public NativeTypeInfo(SIPlusNative.TypeInfoHandle handle) {
            Handle = handle;
        }

        public string Name {
            get {
                unsafe {
                    Util.AssertSuccess(SIPlusNative.siplus_type_name(out var name, Handle));
                    return name.Value ?? "";
                }
            }
        }

        public object? Access(object? value, string name) {
            var inData = Util.MakeData(value);

            Util.AssertSuccess(
                SIPlusNative.siplus_type_access(out var outData, Handle, inData, name)
            );

            return Util.FromData(outData);
        }

        public bool IsIterable(object? value) {
            var inData = Util.MakeData(value);

            Util.AssertSuccess(
                SIPlusNative.siplus_type_is_iterable(out var isIterable, Handle, inData)
            );

            return isIterable;
        }

        public IEnumerator Iterate(object? value) {
            var inData = Util.MakeData(value);

            Util.AssertSuccess(
                SIPlusNative.siplus_type_iterate(out var iterator, Handle, inData)
            );

            return new IteratorEnumerator(new NativeIterator(iterator));
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (Handle != null && !Handle.IsInvalid) {
                Handle.Dispose();
            }
        }

        ~NativeTypeInfo() {
            Dispose(false);
        }
    }
}
