using SIPlus.NET.Internal.Extensions;
using SIPlus.NET.Internal;
using SIPlus.NET.Internal.Extensions;

namespace SIPlus.NET
{
    public class ParserContext : IDisposable
    {
        internal SIPlusNative.ContextHandle _handle;

        internal ParserContext(SIPlusNative.ContextHandle handle) { _handle = handle; }

        public ParserContext UseSTL() {
            Util.AssertSuccess(SIPlusNative.siplus_context_use_stl(_handle));
            AddConverter(new NETStringConverter());
            return this;
        }

        public ParserContext AddFunction(string name, IFunction func) {
            var native = func.ToNativeFunction();
            int result = SIPlusNative.siplus_context_add_function(_handle, name, native.Handle);
            return this;
        }

        public ParserContext AddConverter(IDataConverter converter) {
            var native = converter.ToNativeConverter();
            int result = SIPlusNative.siplus_context_add_converter(_handle, native.Handle);
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
