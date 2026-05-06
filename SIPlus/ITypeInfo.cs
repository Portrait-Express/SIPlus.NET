using System.Collections;

namespace SIPlus {
    public interface ITypeInfo : IDisposable {
        string Name { get; }

        bool IsIterable(object? value);
        object? Access(object? value, string name);
        IEnumerator Iterate(object? value);
    }
}
