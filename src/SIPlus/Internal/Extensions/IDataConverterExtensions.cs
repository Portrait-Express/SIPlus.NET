using SIPlus.NET.Internal;
using SIPlus.NET.Internal.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SIPlus.NET.Internal.Extensions {
    public static class IDataConverterExtensions {
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int ConverterCanConvert(nint result, nint data, nint from, nint to) {
            var thisVal = GCHandle.FromIntPtr(data).Target as IDataConverter;
            if (thisVal == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVALID_ARG,
                    $"{GCHandle.FromIntPtr(data).Target?.GetType()} was not an {nameof(IDataConverter)}");
            }

            var fromType = new SIPlusNative.TypeInfoHandle(from).FromNativeType();
            var toType = new SIPlusNative.TypeInfoHandle(to).FromNativeType();

            try {
                var ptr = (int*)result;
                *ptr = thisVal.CanConvert(fromType, toType) ? 1 : 0;
                return (int)SIPlusNative.Errors.SIPLUS_OK;
            } catch (Exception e) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, e.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int ConverterConvert(nint* result, nint data, nint from, nint to) {
            var thisVal = GCHandle.FromIntPtr(data).Target as IDataConverter;
            if (thisVal == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVALID_ARG,
                    $"{GCHandle.FromIntPtr(data).Target?.GetType()} was not an {nameof(IDataConverter)}");
            }

            var fromVal = new SIValue(new SIPlusNative.DataContainerHandle(from, false));
            var toType = new SIPlusNative.TypeInfoHandle(to).FromNativeType();

            try {
                var handle = thisVal.Convert(fromVal, toType).ToNative();

                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();

                return (int)SIPlusNative.Errors.SIPLUS_OK;
            } catch (Exception e) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, e.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe void ConverterDelete(nint data) {
            var handle = GCHandle.FromIntPtr(data);
            (handle.Target as IDataConverter)?.Dispose();
            GlobalStaticStorage.Release(handle);
            handle.Free();
        }

        internal static unsafe NativeConverter ToNativeConverter(this IDataConverter converter) {
            if (converter is NativeConverter nc) return nc;

            var data = GCHandle.Alloc(converter);
            GlobalStaticStorage.Store(data);

            Util.AssertSuccess(SIPlusNative.siplus_converter_new(out var handle,
                GCHandle.ToIntPtr(data), &ConverterCanConvert, 
                &ConverterConvert, &ConverterDelete));

            return new NativeConverter(handle);
        }

        internal static unsafe IDataConverter FromNativeConverter(this SIPlusNative.ConverterHandle handle) {
            return new NativeConverter(handle);
        }
    }
}
