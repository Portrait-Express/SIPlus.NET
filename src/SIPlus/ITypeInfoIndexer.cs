using System.Collections;

namespace SIPlus.NET {
    public interface ITypeInfoIndexer : IDisposable {
        object? Index(ParserContext context, object? list, object? index);
    }
}
