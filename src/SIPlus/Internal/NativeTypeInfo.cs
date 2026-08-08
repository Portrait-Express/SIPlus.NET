using System.Collections;

namespace SIPlus.NET.Internal {
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

        public SIValue Access(object? value, string name) {
            Util.AssertSuccess(
                SIPlusNative.siplus_type_access(out var outData, Handle, AssertContainerHandle(value), name)
            );

            return new(outData);
        }

        public bool IsIterable(object? value) {
            Util.AssertSuccess(
                SIPlusNative.siplus_type_is_iterable(out var isIterable, Handle, AssertContainerHandle(value))
            );

            return isIterable;
        }

        public IEnumerator<SIValue> Iterate(object? value) {
            Util.AssertSuccess(
                SIPlusNative.siplus_type_iterate(out var iterator, Handle, AssertContainerHandle(value))
            );

            return new IteratorEnumerator(new NativeIterator(iterator));
        }

        private SIPlusNative.DataContainerHandle AssertContainerHandle(object? value) {
            if (value is not SIPlusNative.DataContainerHandle dataHandle) {
                throw new SIPlusException(
                    "NativeTypeInfo did not receive a DataContainerHandle. This likely " +
                    "ocurred by passing an arbitrary object to a SIValue.Type instance. " +
                    "ITypeInfo functions should only be called directly with objects they " +
                    "were paired with in a SIValue, unless you directly control the ITypeInfo."
                );
            }
            return dataHandle;
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
