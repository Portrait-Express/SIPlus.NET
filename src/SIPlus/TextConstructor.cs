using SIPlus.NET;
using SIPlus.NET.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET
{
    public class TextConstructor : IDisposable
    {
        internal SIPlusNative.TextConstructorHandle _handle;

        internal TextConstructor(SIPlusNative.TextConstructorHandle handle)
        {
            _handle = handle; 
        }

        public string Construct(InvocationContext context)
        {
             var result = SIPlusNative.siplus_text_construct(out var resultHandle, _handle, context.Handle);
            Util.AssertSuccess(result);

            return resultHandle.Value ?? "";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_handle != null && !_handle.IsInvalid)
            {
                _handle.Dispose();
            }
        }

        ~TextConstructor() {
            Dispose(false);
        }
    }
}
