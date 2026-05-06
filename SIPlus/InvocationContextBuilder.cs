using SIPlus.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus {
    public class InvocationContextBuilder : IDisposable {
        internal SIPlusNative.InvocationContextBuilderHandle Handle { get; init; }

        internal InvocationContextBuilder(SIPlusNative.InvocationContextBuilderHandle handle ) {
            Handle = handle;
        }

        public InvocationContextBuilder Default(object? data) {
            Util.AssertSuccess(
                SIPlusNative.siplus_icbuilder_default(Handle, Util.MakeData(data))
            );

            return this;
        }

        public InvocationContextBuilder With(string name, object? data) {
            Util.AssertSuccess(
                SIPlusNative.siplus_icbuilder_with(Handle, name, Util.MakeData(data))
            );

            return this;
        }

        public InvocationContext Build() {
            Util.AssertSuccess(
                SIPlusNative.siplus_icbuilder_build(out var context, Handle)
            );

            return new InvocationContext(context);
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

        ~InvocationContextBuilder() {
            Dispose(false);
        }
    }
}
