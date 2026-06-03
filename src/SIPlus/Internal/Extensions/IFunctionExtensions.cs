using SIPlus.NET;
using SIPlus.NET.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET.Internal.Extensions {
    internal static class IFunctionExtensions {
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe int FunctionValue(IntPtr* result, IntPtr thisData, IntPtr parent, int paramc, IntPtr* paramv) {
            var thisVal = GCHandle.FromIntPtr(thisData).Target as IFunction;
            if (thisVal == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVALID_ARG,
                    $"{GCHandle.FromIntPtr(thisData).Target?.GetType()} was not an IFunction");
            }

            IValueRetriever? parentValue = parent == 0 ? null 
                : new SIPlusNative.ValueRetrieverHandle(parent).FromNativeRetriever();
            List<IValueRetriever> parameters = new List<IValueRetriever>();
            for (int i = 0; i < paramc; i++) {
                parameters.Add(new SIPlusNative.ValueRetrieverHandle(paramv[i]).FromNativeRetriever());
            }

            try {
                var value = thisVal.Value(parentValue, parameters);
                var handle = value.ToNativeRetriever().Handle;
                handle.DangerousReleaseHandle();
                *result = handle.DangerousGetHandle();

                return (int) SIPlusNative.Errors.SIPLUS_OK;
            } catch(Exception e) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, e.Message);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static unsafe void FunctionDelete(nint data) {
            var handle = GCHandle.FromIntPtr((IntPtr)data);
            (handle.Target as IFunction)?.Dispose();
            GlobalStaticStorage.Release(handle);
            handle.Free();
        }

        public static unsafe NativeFunction ToNativeFunction(this IFunction function) {
            if (function is NativeFunction nf) return nf;

            var data = GCHandle.Alloc(function);
            GlobalStaticStorage.Store(data);

            var err = SIPlusNative.siplus_function_create(out var handle,
                GCHandle.ToIntPtr(data),
                &FunctionValue, &FunctionDelete);
            Util.AssertSuccess(err);

            return new NativeFunction(handle);
        }
    }
}
