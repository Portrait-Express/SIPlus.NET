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
            throw new NotImplementedException();
        }

        public void Next() {
            throw new NotImplementedException();
        }

        public object? Current() {
            throw new NotImplementedException();
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
    }
}
