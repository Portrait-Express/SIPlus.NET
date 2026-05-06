using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SIPlus.Internal.Extensions {
    internal static class IIteratorExtensions {
        private static int More(nint thisData) {
            var iterator = GCHandle.FromIntPtr(thisData).Target as IIterator;

            if (iterator == null) {
                return 0;
            }

            return iterator.More() ? 1 : 0;
        }

        private static int Next(nint thisData) {
            var iterator = GCHandle.FromIntPtr(thisData).Target as IIterator;

            if (iterator == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_ERR, 
                    "iterator was null");
            }

            try {
                iterator.Next();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch(Exception ex) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_ERR,
                    ex.ToString());
            }
        }

        private static unsafe int Current(nint *result, nint thisData) {
            var iterator = GCHandle.FromIntPtr(thisData).Target as IIterator;

            if (iterator == null) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_ERR,
                    "iterator was null");
            }

            try {
                var handle = Util.MakeData(iterator.Current());
                *result = handle.DangerousGetHandle();
                handle.DangerousReleaseHandle();

                return SIPlusNative.siplus_error_set((int)SIPlusNative.Errors.SIPLUS_OK, "");
            } catch (Exception ex) {
                return SIPlusNative.siplus_error_set(
                    (int)SIPlusNative.Errors.SIPLUS_ERR,
                    ex.ToString());
            }
        }

        private static void Delete(nint thisData) {
            var handle = GCHandle.FromIntPtr(thisData);
            (handle.Target as IIterator)?.Dispose();
            GlobalStaticStorage.Release(handle);
            handle.Free();
        }

        public static unsafe NativeIterator ToNativeIterator(this IIterator iterator) {
            if (iterator is NativeIterator ni) return ni;

            var objHandle = GCHandle.Alloc(iterator);
            GlobalStaticStorage.Store(objHandle);

            Util.AssertSuccess(
                SIPlusNative.siplus_iterator_new(out var iteratorHandle, 
                    GCHandle.ToIntPtr(objHandle), More, Next, Current, Delete));

            return new NativeIterator(iteratorHandle);
        }
    }
}
