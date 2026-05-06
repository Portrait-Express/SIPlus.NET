using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus
{
    public class InvocationException : SIPlusException
    {
        internal InvocationException(string message) : base(message) { }
    }
}
