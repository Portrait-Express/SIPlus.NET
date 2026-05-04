using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus {
    public interface IFunction : IDisposable {
        public IValueRetriever Value(IValueRetriever? parent, List<IValueRetriever> parameters);
    }
}
