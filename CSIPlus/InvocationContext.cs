using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus
{
    public class InvocationContext : IDisposable
    {
        internal SIPlusNative.InvocationContextHandle Handle;

        internal InvocationContext(SIPlusNative.InvocationContextHandle handle) {
            Handle = handle;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Handle != null && !Handle.IsInvalid)
            {
                Handle.Dispose();
            }
        }
    }
}
