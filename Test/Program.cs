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
            formatter.Append("BLAH!");
        }
    }

    class Program {
        static readonly string formatTest = "Foo {0,13:e} and bar!! {1,-15:P}bah";

        static void Main (string[] args) {
            var formatter = new StringFormatter();
            var v1 = -13.634509999999999999 * 999999;
            var v2 = 9;

            formatter.AppendFormat(formatTest, v1, v2);
            Console.WriteLine(formatter.ToString());

            Console.WriteLine(formatTest, v1, v2);
            Console.ReadLine();
        }
    }
}
