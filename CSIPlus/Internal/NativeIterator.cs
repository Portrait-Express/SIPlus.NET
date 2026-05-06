using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal {
    internal class NativeIterator : IIterator {
        internal SIPlusNative.IteratorHandle Handle { get; init; }

        public NativeIterator(SIPlusNative.IteratorHandle handle) { 
            Handle = handle;
        }

        public bool More() {
            Util.AssertSuccess(SIPlusNative.siplus_iterator_more(out bool result, Handle));
            return result;
        }

        public void Next() {
            Util.AssertSuccess(SIPlusNative.siplus_iterator_next(Handle));
        }

        public object? Current() {
            Util.AssertSuccess(SIPlusNative.siplus_iterator_current(out var result, Handle));
            return Util.FromData(result);
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

        ~NativeIterator() {
            Dispose(false);
        }
    }
}
