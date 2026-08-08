using SIPlus.NET.Internal.Extensions;

namespace SIPlus.NET.Internal {
    internal class NativeConverter : IDataConverter {
        internal SIPlusNative.ConverterHandle Handle { get; init; }

        public NativeConverter(SIPlusNative.ConverterHandle handle) { 
            Handle = handle;
        }

        public bool CanConvert(ITypeInfo from, ITypeInfo to) {
            Util.AssertSuccess(
                SIPlusNative.siplus_converter_can_convert(
                    out var can, Handle,
                    from.GetNativeTypeInfo().Handle,
                    to.GetNativeTypeInfo().Handle));

            return can;
        }

        public SIValue Convert(SIValue from, ITypeInfo to) {
            Util.AssertSuccess(
                SIPlusNative.siplus_converter_convert(
                    out var result, Handle,
                    from.ToNative(),
                    to.GetNativeTypeInfo().Handle));

            return new(result);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if(Handle != null && !Handle.IsInvalid) {
                Handle.Dispose();
            }
        }

        ~NativeConverter() {
            Dispose(false);
        }
    }
}
