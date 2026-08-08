using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET.Internal {
    internal interface IIterator : IDisposable {
        bool More();
        void Next();
        SIValue Current();
    }
}
