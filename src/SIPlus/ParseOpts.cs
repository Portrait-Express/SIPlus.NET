using SIPlus.NET;
using SIPlus.NET.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET
{
    public class ParseOpts : IDisposable
    {
        internal SIPlusNative.ParseOptsHandle Handle;

        public ParseOpts()
        {
            Handle = SIPlusNative.siplus_parse_opts_new();
        }

        public ParseOpts AddGlobal(string name) {
            Util.AssertSuccess(SIPlusNative.siplus_parse_opts_add_global(Handle, name));
            return this;
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

        ~ParseOpts() {
            Dispose(false);
        }
    }
}
