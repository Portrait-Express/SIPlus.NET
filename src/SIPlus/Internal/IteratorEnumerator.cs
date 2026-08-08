using System.Collections;

namespace SIPlus.NET.Internal {
    internal class IteratorEnumerator : IEnumerator<SIValue> {
        private IIterator _iterator;

        public IteratorEnumerator(IIterator iterator) {
            _iterator = iterator;
        }

        public SIValue Current => _iterator.Current();
        object IEnumerator.Current => Current;

        public bool MoveNext() {
            if (!_iterator.More()) return false;
            _iterator.Next();
            return true;
        }

        public void Reset() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(disposing) {
                _iterator.Dispose();
            }
        }

        ~IteratorEnumerator() {
            Dispose(false);
        }
    }
}
