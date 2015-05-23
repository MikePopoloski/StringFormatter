using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Formatting;
using System.Threading.Tasks;

namespace Test {
    struct Blah : IStringifiable {
        public void Format (StringFormatter formatter) {
            formatter.Append("BLAH!");
        }
    }

    class Program {

        static void Main (string[] args) {
            var formatter = new StringFormatter();
            Blah b;
            formatter.Append("Foo {0,3} and bar!! {1,-5}bah", 3, b);

            Console.WriteLine(formatter.ToString());
        }
    }
}
