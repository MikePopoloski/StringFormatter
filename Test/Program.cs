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

        const double v1 = 13.934939;
        const double v2 = 0;

        static void Main (string[] args) {
            var f = new StringFormatter();
            f.AppendFormat(formatTest, v1, v2);
            Console.WriteLine(f.ToString());
            Console.WriteLine(formatTest, v1, v2);

            PerfTest();
#if DEBUG
            Console.ReadLine();
#endif
        }

        static void PerfTest () {
            var formatter = new StringFormatter();
            var builder = new StringBuilder();

            GC.Collect(2, GCCollectionMode.Forced, true);
            var gcCount = GC.CollectionCount(0);
            var timer = Stopwatch.StartNew();
            
            for (int k = 0; k < mul; k++) {
                for (int i = 0; i < count; i++)
                    formatter.AppendFormat(formatTest, v1, v2);
                formatter.Clear();
            }
            timer.Stop();
            Console.WriteLine("Mine : {0} us/format", timer.ElapsedMilliseconds * 1000.0 / (count * mul));
            Console.WriteLine("GCs  : {0}", GC.CollectionCount(0) - gcCount);
            Console.WriteLine();

            GC.Collect(2, GCCollectionMode.Forced, true);
            gcCount = GC.CollectionCount(0);
            timer = Stopwatch.StartNew();
            
            for (int k = 0; k < mul; k++) {
                for (int i = 0; i < count; i++)
                    builder.AppendFormat(formatTest, v1, v2);
                builder.Clear();
            }
            timer.Stop();
            Console.WriteLine("BCL  : {0} us/format", timer.ElapsedMilliseconds * 1000.0 / (count * mul));
            Console.WriteLine("GCs  : {0}", GC.CollectionCount(0) - gcCount);
        }
    }
}
