using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus
{
    public class SIPlusException : Exception
    {
        internal SIPlusException(string message) : base(message) { }
    }
}
