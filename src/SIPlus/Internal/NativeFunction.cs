using SIPlus.NET.Internal.Extensions;
using SIPlus.NET.Internal;

namespace SIPlus.NET.Internal {
    internal class NativeFunction : IFunction {
        internal SIPlusNative.FunctionHandle Handle { get; init; }

        public NativeFunction(SIPlusNative.FunctionHandle handle) {
            Handle = handle;
        }

        public IValueRetriever Value(IValueRetriever? parent, List<IValueRetriever> parameters) {
            var nativeParent = parent?.ToNativeRetriever();
            var nativeParameters = parameters.Select(p => p.ToNativeRetriever());
            var err = SIPlusNative.siplus_function_value(
                out var retriever, 
                Handle, 
                nativeParent?.Handle.DangerousGetHandle() ?? 0,
                parameters.Count,
                nativeParameters.Select(v => v.Handle.DangerousGetHandle()).ToArray());
            Util.AssertSuccess(err);

            return retriever.FromNativeRetriever();
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

        ~NativeFunction() {
            Dispose(false);
        }
    }
}
