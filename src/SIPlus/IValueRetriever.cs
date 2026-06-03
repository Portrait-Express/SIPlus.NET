using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET
{
    public interface IValueRetriever : IDisposable
    {
        public object? Retrieve(InvocationContext context);
    }
}
