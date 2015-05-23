using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Formatting;
using System.Threading.Tasks;

namespace Test {
    struct Blah : IStringFormattable {
        public void Format (StringFormatter formatter, StringView format) {
            formatter.Append("BLAH!", format);
        }
    }

    class Program {
        static readonly string formatTest = "Foo {0,13:x} and bar!! {1,-15:P}bah";

        static void Main (string[] args) {
            var formatter = new StringFormatter();
            Blah b;
            formatter.AppendFormat(formatTest, -3000, 9);

            Console.WriteLine(formatter.ToString());

            Console.WriteLine(formatTest, -3000, 9);
            Console.ReadLine();
        }
    }
}
