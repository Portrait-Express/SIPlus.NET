using SIPlus.NET.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SIPlus.NET.Internal.Extensions {
    internal static class ITypeInfoExtensions {
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe int IsIterable(nint thisData, nint obj) {
            var info = GCHandle.FromIntPtr(thisData).Target as ITypeInfo;
            var data = GCHandle.FromIntPtr(obj).Target;

            if (data == null || info == null) {
                return 0;
            }

            try {
                return info.IsIterable(data) ? 1 : 0;
            } catch (Exception ex) {
                return 0;
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe int Access(nint *result, nint thisData, nint obj, nint name) {
            var info = GCHandle.FromIntPtr(thisData).Target as ITypeInfo;
            var data = GCHandle.FromIntPtr(obj).Target;

            if (info == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, 
                    "access: Type info was null");
            }

            try {
                string nameStr = Marshal.PtrToStringAnsi(name) ?? 
                    throw new InvalidOperationException("No name passed to 'access'");

                var handle = info.Access(data, nameStr).ToNative();

                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, ex.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe int Index(nint* result, nint thisData, nint context, nint value, nint index) {
            var valueobj = GCHandle.FromIntPtr(value).Target;
            var indexobj = new SIValue(new(index, false));

            if (GCHandle.FromIntPtr(thisData).Target is not ITypeInfoIndexer info) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR,
                    "index: Type info was null");
            }

            try {
                var contextHandle = new SIPlusNative.ContextHandle(context, false);
                var handle = info.Index(new ParserContext(contextHandle), valueobj, indexobj)
                    .ToNative();

                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, ex.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe int Iterate(nint *result, nint thisData, nint obj) {
            var info = GCHandle.FromIntPtr(thisData).Target as ITypeInfo;
            var data = GCHandle.FromIntPtr(obj).Target;

            if (info == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR,
                    "iterate: Type info was null");
            }


            try {
                var enumerator = info.Iterate(data);
                var iterator = new EnumeratorIterator(enumerator).ToNativeIterator();

                *result = iterator.Handle.DangerousGetHandle();
                iterator.Handle.DangerousReleaseHandle();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, ex.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void Delete(nint thisData) {
            var handle = GCHandle.FromIntPtr(thisData);
            (handle.Target as ITypeInfo)?.Dispose();
            GlobalStaticStorage.Release(handle);
            handle.Free();
        }

        internal static unsafe NativeTypeInfo GetNativeTypeInfo(this ITypeInfo info) {
            if (info is NativeTypeInfo nti) return nti;

            var handle = GCHandle.Alloc(info);
            GlobalStaticStorage.Store(handle);

            int err;
            var ptr = Marshal.StringToHGlobalAnsi(info.Name);

            try {
                var args = new SIPlusNative.SIPlusTypeInfoData() {
                    data = GCHandle.ToIntPtr(handle),
                    name = ptr,
                    is_iterable = &IsIterable,
                    access = &Access,
                    index = info is ITypeInfoIndexer ? &Index : null,
                    iterate = &Iterate,
                    delete = &Delete
                };

                err = SIPlusNative.siplus_type_new_s(out var typeHandle, args);
                Util.AssertSuccess(err);
                return new NativeTypeInfo(typeHandle);
            } finally {
                Marshal.FreeHGlobal(ptr);
            }
        }

        internal static unsafe ITypeInfo FromNativeType(this SIPlusNative.TypeInfoHandle handle) {
            return new NativeTypeInfo(handle);
        }
    }
}