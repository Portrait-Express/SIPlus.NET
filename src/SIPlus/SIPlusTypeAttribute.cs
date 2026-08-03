using System;
using System.Collections.Generic;
using System.Text;

namespace SIPlus.NET {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SIPlusTypeAttribute(Type typeInfoType) : Attribute {
        public Type TypeInfoType = typeInfoType;
    }
}
