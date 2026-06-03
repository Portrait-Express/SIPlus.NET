using SIPlus.NET;

namespace SIPlus.Test {
    internal class Util {
        internal static Parser GetBaseTestingParser() {
            Parser parser = new();
            parser.Context().UseSTL();

            return parser;
        }
    }
}
