using System.Collections;

namespace SIPlus.NET {
    public interface ITypeInfo : IDisposable {
        string Name { get; }

        bool IsIterable(object? value);
        SIValue Access(object? value, string name);
        IEnumerator<SIValue> Iterate(object? value);
    }
}
