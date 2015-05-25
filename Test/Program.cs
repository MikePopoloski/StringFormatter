using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        const int count = 1000000;
        const int mul = 5;
        static readonly string formatTest = "Foo {0,13:e12} and bar!! {1,-15:P}bah";

        static void Main (string[] args) {


            var v1 = 13.934987234987234987234m;
            //var v1 = -14;
            var v2 = 9;

            var gcCount = GC.CollectionCount(0);

            var count = 1000000;
            var timer = Stopwatch.StartNew();

            for (int k = 0; k < mul; k++) {
                var formatter = new StringFormatter();
                for (int i = 0; i < count; i++)
                    formatter.AppendFormat(formatTest, v1, v2);
            }
            timer.Stop();
            Console.WriteLine("Mine : {0} us/format", timer.ElapsedMilliseconds * 1000.0 / (count * mul));
            Console.WriteLine("GCs  : {0}", GC.CollectionCount(0) - gcCount);
            Console.WriteLine();

            GC.Collect(2, GCCollectionMode.Forced, true);
            gcCount = GC.CollectionCount(0);

            timer = Stopwatch.StartNew();
            for (int k = 0; k < mul; k++) {
                var builder = new StringBuilder();
                for (int i = 0; i < count; i++)
                    builder.AppendFormat(formatTest, v1, v2);
            }
            timer.Stop();
            Console.WriteLine("BCL  : {0} us/format", timer.ElapsedMilliseconds * 1000.0 / (count * mul));
            Console.WriteLine("GCs  : {0}", GC.CollectionCount(0) - gcCount);

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
