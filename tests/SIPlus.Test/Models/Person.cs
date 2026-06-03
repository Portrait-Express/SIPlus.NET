using System;
using System.Collections.Generic;
using System.Text;

namespace SIPlus.Test.Models {
    public class Person {
        public string FirstName = "John";
        public string LastName = "Doe";

        public override string ToString() {
            return $"{FirstName} {LastName}";
        }
    }
}
