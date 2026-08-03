using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SIPlus.NET.Internal {
    internal class TypeInfoWrapper(ITypeInfo info) : ITypeInfo {
        private ITypeInfo _typeInfo = info;

        public string Name => _typeInfo.Name;

        public object? Access(object? value, string name) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public bool IsIterable(object? value) {
            throw new NotImplementedException();
        }

        public IEnumerator Iterate(object? value) {
            throw new NotImplementedException();
        }
    }
}
