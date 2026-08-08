using SIPlus.NET;
using SIPlus.NET.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET.Internal
{
    internal class NativeValueRetriever : IValueRetriever
    {
        internal SIPlusNative.ValueRetrieverHandle Handle { get; private init; }

        internal NativeValueRetriever(SIPlusNative.ValueRetrieverHandle handle) { 
            Handle = handle;
        }

        public SIValue Retrieve(InvocationContext context)
        {
            Util.AssertSuccess(SIPlusNative.siplus_value_retrieve(out var data, Handle, context.Handle));
            return new(data);
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

        ~NativeValueRetriever() {
            Dispose(false);
        }
    }
}
