using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET {
    public interface IDataConverter : IDisposable {
        bool CanConvert(ITypeInfo from, ITypeInfo to);
        object? Convert(object? from, ITypeInfo to);
    }
}
