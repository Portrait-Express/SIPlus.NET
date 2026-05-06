using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SIPlus.Internal.Extensions {
    internal static class ITypeInfoExtensions {
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

        internal static unsafe int Access(nint *result, nint thisData, nint obj, string name) {
            var info = GCHandle.FromIntPtr(thisData).Target as ITypeInfo;
            var data = GCHandle.FromIntPtr(obj).Target;

            if (info == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, 
                    "access: Type info was null");
            }

            try {
                var handle = Util.MakeData(info.Access(data, name));
                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, ex.ToString());
            }
        }

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
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, ex.ToString());
            }
        }

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

            int err = SIPlusNative.siplus_type_new(out var typeHandle, GCHandle.ToIntPtr(handle), info.Name,
                IsIterable, Access, Iterate, Delete);
            Util.AssertSuccess(err);

            return new NativeTypeInfo(typeHandle);
        }
    }
}