using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET
{
    public class ParseException : SIPlusException
    {
        internal ParseException(string message) : base(message) { }
    }
}
