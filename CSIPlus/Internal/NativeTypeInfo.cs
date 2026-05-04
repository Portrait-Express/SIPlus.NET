using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal {
    internal class NativeTypeInfo : ITypeInfo {
        internal SIPlusNative.TypeInfoHandle Handle;

        public NativeTypeInfo(SIPlusNative.TypeInfoHandle handle) {
            Handle = handle;
        }

        public string Name {
            get {
                throw new NotImplementedException();
            }
        }

        public object? Access(object? value, string name) {
            throw new NotImplementedException();
        }

        public bool IsIterable(object? value) {
            throw new NotImplementedException();
        }

        public IEnumerator Iterate(object? value) {
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
