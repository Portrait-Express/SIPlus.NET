using System.Collections;

namespace SIPlus.NET {
    public interface ITypeInfoIndexer : IDisposable {
        SIValue Index(ParserContext context, object? list, SIValue index);
    }
}
