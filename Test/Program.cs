using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Formatting;
using System.Threading.Tasks;

namespace Test {
    class Program {
        static void Main (string[] args) {
            var formatter = new StringFormatter();
            formatter.Append("Foo {0,3} and bar!! {1,-5}bah", 3, -8);

            Console.WriteLine(formatter.ToString());
        }
    }
}
