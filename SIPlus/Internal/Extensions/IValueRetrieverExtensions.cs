using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.Internal.Extensions {
    internal static class IValueRetrieverExtensions {
        private unsafe static int Retrieve(nint* result, nint data, nint context) {
            IValueRetriever? retriever = GCHandle.FromIntPtr(data).Target as IValueRetriever;
            if (retriever == null) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_ERR, "GCHandle not valid");
            }

            try {
                var handle = Util.MakeData(retriever.Retrieve(new InvocationContext(new(context))));
                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR, ex.Message);
            }
        }

        private unsafe static void Delete(nint data) {
            var handle = GCHandle.FromIntPtr(data);
            (handle.Target as IValueRetriever)?.Dispose();
            GlobalStaticStorage.Release(handle);
            handle.Free();
        }

        public static unsafe NativeValueRetriever ToNativeRetriever(this IValueRetriever valueRetriever) {
            if (valueRetriever is NativeValueRetriever nvr) return nvr;

            var handle = GCHandle.Alloc(valueRetriever);
            GlobalStaticStorage.Store(handle);

            var err = SIPlusNative.siplus_value_create(out var retrieverHandle, GCHandle.ToIntPtr(handle),
                    Retrieve, Delete);
            Util.AssertSuccess(err);

            return new NativeValueRetriever(retrieverHandle);
        }

        public unsafe static IValueRetriever FromNativeRetriever(this SIPlusNative.ValueRetrieverHandle handle) {
            // TODO: Make this check if user-provided and cast back to intiial instance?
            //       Will need siplus_value_get_data to get the void* passed to value_create
            return new NativeValueRetriever(handle); 
        }
    }
}
