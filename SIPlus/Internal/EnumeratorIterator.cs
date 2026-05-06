using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus.Internal {
    internal class EnumeratorIterator : IIterator {
        private IEnumerator _enumerator;

        private object? _last;
        private bool _more;

        public EnumeratorIterator(IEnumerator enumerator) {
            _enumerator = enumerator;

            _more = _enumerator.MoveNext();
        }

        public object? Current() {
            return _last;
        }

        public bool More() {
            return _more;
        }

        public void Next() {
            _last = _enumerator.Current;
            _more = _enumerator.MoveNext();
        }

        public void Dispose() { }
    }
}
