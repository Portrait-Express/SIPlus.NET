using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal {
    internal interface IIterator : IDisposable {
        bool More();
        void Next();
        object? Current();
    }
}
