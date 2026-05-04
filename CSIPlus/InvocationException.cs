using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus
{
    public class InvocationException : SIPlusException
    {
        internal InvocationException(string message) : base(message) { }
    }
}
