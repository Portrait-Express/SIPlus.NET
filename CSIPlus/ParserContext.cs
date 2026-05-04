using CSIPlus.Internal;
using CSIPlus.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus
{
    public class ParserContext
    {
        internal SIPlusNative.ContextHandle _handle;

        internal ParserContext(SIPlusNative.ContextHandle handle) { _handle = handle; }

        public void AddFunction(string name, IFunction func) {
            var native = func.ToNativeFunction();
            int result = SIPlusNative.siplus_context_add_function(_handle, name, native.Handle);
        }

        public InvocationContextBuilder Builder() {
            Util.AssertSuccess(
                SIPlusNative.siplus_context_builder(out var builder, _handle)
            );

            return new InvocationContextBuilder(builder);
        }
    }
}
