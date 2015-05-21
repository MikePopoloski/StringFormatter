using System;

namespace System.Text.Formatting {
    public class StringFormatter {
        char[] buffer;
        int count;

        public StringFormatter ()
            : this(128) {
        }

        public StringFormatter (int capacity) {
            buffer = new char[capacity];
        }

        public void Append (char c) {
            buffer[count++] = c;
        }

        public void Append (char c, int count) {
            for (int i = 0; i < count; i++)
                buffer[count++] = c;
        }

        public void Append (int i) {
            Append(i.ToString());
        }

        public void Append (string str) {
            foreach (var c in str)
                buffer[count++] = c;
        }

        public void Append<T>(T value) {
            // this looks gross, but T is known at JIT-time so this call tree
            // is hopefully going to get optimized down to a direct call
            if (typeof(T) == typeof(int))
                Append(value as int? ?? 0);
            else if (typeof(T) == typeof(string))
                Append(value as string);
            else if (typeof(T) == typeof(char))
                Append(value as char? ?? 0);
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

        public void Append<T0, T1>(string format, T0 arg0, T1 arg1) {
            var args = new Arg2<T0, T1>(arg0, arg1);
            AppendSet(format, ref args);
        }

        public void AppendSet<T>(string format, ref T args) where T : IArgSet {
            unsafe
            {
                fixed (char* formatPtr = format)
                {
                    var curr = formatPtr;
                    var end = curr + format.Length;
                    while (AppendSegment(ref curr, end, ref args)) ;
                }
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

            var currentCount = count;
            args.Format(this, index);

            // finish off padding, if necessary
            var padding = width - (count - currentCount);
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

    public interface IArgSet {
        int Count { get; }
        void Format (StringFormatter formatter, int index);
    }

    // need a better name for this
    interface IStringifiable {
        void Format (StringFormatter formatter);
    }
}