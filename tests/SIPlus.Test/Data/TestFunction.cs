using SIPlus.NET;

namespace SIPlus.Test.Data {
    internal class TestFuncValueRetriever : IValueRetriever {
        public SIValue Retrieve(InvocationContext value) {
            return new("test");
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Just returns "test"
    /// </summary>
    internal class TestFunction : IFunction {
        public IValueRetriever Value(IValueRetriever? parent, List<IValueRetriever> parameters) {
            return new TestFuncValueRetriever();
        }

        public void Dispose() { }
    }
}
