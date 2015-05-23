using System;
using System.Globalization;
using System.Reflection;
using System.Linq;

namespace System.Text.Formatting {
    public class StringFormatter {
        char[] buffer;
        int currentCount;

        public StringFormatter ()
            : this(128) {
        }

        public StringFormatter (int capacity) {
            buffer = new char[capacity];
        }

        public void Append (char c) {
            buffer[currentCount++] = c;
        }

        public void Append (char c, int count) {
            for (int i = 0; i < count; i++)
                buffer[count++] = c;
        }

        public void Append (int i) {
            Numeric.FormatInt32(this, i, "G", CultureInfo.CurrentCulture.NumberFormat);
        }

        public void Append (string str) {
            foreach (var c in str)
                buffer[currentCount++] = c;
        }

        public unsafe void Append (char* str, int count) {
            for (int i = 0; i < count; i++)
                buffer[currentCount++] = *str++;
        }

        internal void AppendGeneric<T>(T value) {
            // this looks gross, but T is known at JIT-time so this call tree
            // gets compiled down to a direct call to the appropriate specialized method
            // we need to use typed references here to convince the compiler that yes,
            // we really do have the value we say we do
            if (typeof(T) == typeof(int))
                Append(__refvalue(__makeref(value), int));
            else if (typeof(T) == typeof(string))
                Append(__refvalue(__makeref(value), string));
            else if (typeof(T) == typeof(char))
                Append(__refvalue(__makeref(value), char));
            else if (ValueHelper<T>.Thingy != null) {
                ValueHelper<T>.Thingy(this, value);
            }
            else
                throw new InvalidOperationException();
        }

        public override string ToString () {
            return string.Concat(buffer);
        }

        public void Append<T0>(string format, T0 arg0) {
            var args = new Arg1<T0>(arg0);
            AppendSet(format, ref args);
        }

        public unsafe void Append<T0, T1>(string format, T0 arg0, T1 arg1) {
            var args = new Arg2<T0, T1>(arg0, arg1);
            AppendSet(format, ref args);
        }

        public unsafe void AppendSet<T>(string format, ref T args) where T : IArgSet {
            fixed (char* formatPtr = format)
            {
                var curr = formatPtr;
                var end = curr + format.Length;
                while (AppendSegment(ref curr, end, ref args)) ;
            }
        }

        unsafe bool AppendSegment<T>(ref char* currRef, char* end, ref T args) where T : IArgSet {
            char* curr = currRef;
            char c = '\x0';
            while (curr < end) {
                c = *curr++;
                if (c == '}') {
                    // check for escape character for }}
                    if (curr < end && *curr == '}')
                        curr++;
                    else
                        ThrowError();
                }

                if (c == '{') {
                    // check for escape character for {{
                    if (curr == end)
                        ThrowError();
                    else if (*curr == '{')
                        curr++;
                    else
                        break;
                }

                Append(c);
            }

            if (curr == end)
                return false;

            var index = ParseNum(ref curr, end, MaxArgs);
            if (index >= args.Count)
                ThrowError();

            // check for a spacing specifier
            c = SkipWhitespace(ref curr, end);
            int width = 0;
            var leftJustify = false;
            if (c == ',') {
                curr++;
                c = SkipWhitespace(ref curr, end);

                // spacing can be left-justified
                if (c == '-') {
                    leftJustify = true;
                    curr++;
                    if (curr == end)
                        ThrowError();
                }

                width = ParseNum(ref curr, end, MaxSpacing);
                c = SkipWhitespace(ref curr, end);
            }

            // check for format specifier

            if (c != '}')
                ThrowError();
            curr++;
            currRef = curr;

            var currentCount = this.currentCount;
            args.Format(this, index);

            // finish off padding, if necessary
            var padding = width - (this.currentCount - currentCount);
            if (padding > 0) {
            }

            return true;
        }

        unsafe static int ParseNum (ref char* currRef, char* end, int maxValue) {
            char* curr = currRef;
            char c = *curr;
            if (c < '0' || c > '9')
                ThrowError();

            int value = 0;
            do {
                value = value * 10 + c - '0';
                curr++;
                if (curr == end)
                    ThrowError();
                c = *curr;
            } while (c >= '0' && c <= '9' && value < maxValue);

            currRef = curr;
            return value;
        }

        unsafe static char SkipWhitespace (ref char* currRef, char* end) {
            char* curr = currRef;
            while (curr < end && *curr == ' ') curr++;

            if (curr == end)
                ThrowError();

            currRef = curr;
            return *curr;
        }

        static void ThrowError () {
            throw new FormatException();
        }

        const int MaxArgs = 64;
        const int MaxSpacing = 1000000;
    }

    static class ValueHelper<T> {
        static readonly MethodInfo Assigner = typeof(ValueHelper<T>).GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Static);

        public static readonly Action<StringFormatter, T> Thingy = Prepare();

        static Action<StringFormatter, T> Prepare () {
            var type = typeof(T);
            if (!type.IsValueType || !typeof(IStringifiable).IsAssignableFrom(type))
                return null;

            var a = Assigner;
            return (Action<StringFormatter, T>)a.MakeGenericMethod(type).Invoke(null, null);
        }

        static Action<StringFormatter, U> Assign<U>() where U : IStringifiable {
            return (f, t) => t.Format(f);
        }


    }

    //unsafe struct Arg2cc<T0, T1> : IArgSet {
    //    IntPtr t0;
    //    IntPtr t1;

    //    public int Count => 2;

    //    public Arg2cc (TypedReference t0, TypedReference t1) {
    //        this.t0 = *(IntPtr*)&t0;
    //        this.t1 = *(IntPtr*)&t1;
    //    }

    //    public void Format (StringFormatter formatter, int index) {


    //        Accessors[index](formatter, ref this);
    //    }
    //}

    //static class BufferPacker<T0, T1> {
    //    public static readonly int 
    //}

    public interface IArgSet {
        int Count { get; }
        void Format (StringFormatter formatter, int index);
    }

    // need a better name for this
    public interface IStringifiable {
        void Format (StringFormatter formatter);
    }
}