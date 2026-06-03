using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.NET.Internal {
    internal class IteratorEnumerator : IEnumerator {
        private IIterator _iterator;

        public IteratorEnumerator(IIterator iterator) {
            _iterator = iterator;
        }

        public object? Current => _iterator.Current();

        public bool MoveNext() {
            if (!_iterator.More()) return false;
            _iterator.Next();
            return true;
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
