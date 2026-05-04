using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal
{
    internal static class GlobalStaticStorage
    {
        private static HashSet<GCHandle> _handles = new();

        public static void Store(GCHandle handle)
        {
            _handles.Add(handle);
        }

        public static void Release(GCHandle handle)
        {
            _handles.Remove(handle);
        }
    }
}
