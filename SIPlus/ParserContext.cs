using SIPlus.Internal;
using SIPlus.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus
{
    public class ParserContext : IDisposable
    {
        internal SIPlusNative.ContextHandle _handle;

        internal ParserContext(SIPlusNative.ContextHandle handle) { _handle = handle; }

        public ParserContext UseSTL() {
            Util.AssertSuccess(SIPlusNative.siplus_context_use_stl(_handle));
            return this;
        }

        public ParserContext AddFunction(string name, IFunction func) {
            var native = func.ToNativeFunction();
            int result = SIPlusNative.siplus_context_add_function(_handle, name, native.Handle);
            return this;
        }

        public InvocationContextBuilder Builder() {
            Util.AssertSuccess(
                SIPlusNative.siplus_context_builder(out var builder, _handle)
            );

            return new InvocationContextBuilder(builder);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing) {
            if(_handle != null && !_handle.IsInvalid) {
                _handle.Dispose();
            }
        }

        ~ParserContext() {
            Dispose(false);
        }
    }
}
