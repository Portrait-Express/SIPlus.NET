using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus
{
    public class ParseException : SIPlusException
    {
        internal ParseException(string message) : base(message) { }
    }
}
