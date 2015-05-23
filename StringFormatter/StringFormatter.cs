using System;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace System.Text.Formatting {
    public unsafe class StringFormatter {
        CachedCulture culture;
        char[] buffer;
        int currentCount;
        List<char> specifierBuffer;

        public StringFormatter ()
            : this(128) {
        }

        public StringFormatter (int capacity) {
            buffer = new char[capacity];
            culture = new CachedCulture(CultureInfo.CurrentCulture.NumberFormat);

            specifierBuffer = new List<char>();
        }

        public void Append (char c) {
            buffer[currentCount++] = c;
        }

        public void Append (char c, int count) {
            for (int i = 0; i < count; i++)
                buffer[currentCount++] = c;
        }

        public void Append (int value, StringView format) {
            Numeric.FormatInt32(this, value, format, culture);
        }

        public void Append (uint value, StringView format) {
        }

        public void Append (long value, StringView format) {
        }

        public void Append (ulong value, StringView format) {
        }

        public void Append (float value, StringView format) {
        }

        public void Append (double value, StringView format) {
        }

        public void Append (decimal value, StringView format) {
        }

        public void Append (bool value, StringView format) {
        }

        public void Append (string str, StringView format) {
            foreach (var c in str)
                buffer[currentCount++] = c;
        }

        public unsafe void Append (char* str, int count) {
            for (int i = 0; i < count; i++)
                buffer[currentCount++] = *str++;
        }

        public override string ToString () {
            return string.Concat(buffer);
        }

        public void AppendFormat<T0>(string format, T0 arg0) {
            var args = new Arg1<T0>(__makeref(arg0));
            AppendArgSet(format, ref args);
        }

        public void AppendFormat<T0, T1>(string format, T0 arg0, T1 arg1) {
            var args = new Arg2<T0, T1>(__makeref(arg0), __makeref(arg1));
            AppendArgSet(format, ref args);
        }

        public void AppendArgSet<T>(string format, ref T args) where T : IArgSet {
            fixed (char* formatPtr = format)
            {
                var curr = formatPtr;
                var end = curr + format.Length;
                while (AppendSegment(ref curr, end, ref args)) ;
            }
        }

        bool AppendSegment<T>(ref char* currRef, char* end, ref T args) where T : IArgSet {
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
                else if (c == '{') {
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
            var specifier = new StringView();
            if (c == ':') {
                specifierBuffer.Clear();
                curr++;
                while (true) {
                    if (curr == end)
                        ThrowError();

                    c = *curr++;
                    if (c == '{') {
                        // check for escape character for {{
                        if (curr < end && *curr == '{')
                            curr++;
                        else
                            ThrowError();
                    }
                    else if (c == '}') {
                        // check for escape character for }}
                        if (curr < end && *curr == '}')
                            curr++;
                        else {
                            curr--;
                            break;
                        }
                    }

                    specifierBuffer.Add(c);
                }

                specifier.Data = string.Concat(specifierBuffer);
            }

            if (c != '}')
                ThrowError();
            curr++;
            currRef = curr;

            var oldCount = currentCount;
            args.Format(this, index, specifier);

            // finish off padding, if necessary
            var padding = width - (currentCount - oldCount);
            if (padding > 0) {
                if (leftJustify)
                    Append(' ', padding);
                else {
                    // copy the recently placed chars up in memory to make room for padding
                    for (int i = currentCount - 1; i >= oldCount; i--)
                        buffer[i + padding] = buffer[i];

                    // fill in padding
                    for (int i = 0; i < padding; i++)
                        buffer[i + oldCount] = ' ';
                    currentCount += padding;
                }
            }

            return true;
        }

        internal void AppendGeneric<T>(IntPtr ptr, StringView format) {
            // ptr here is a pointer to the parameter we want to format; for
            // simple value types we can cast the pointer directly, but for
            // strings and unknown generic value types we need to pull them
            // out via a typed reference

            // this looks gross, but T is known at JIT-time so this call tree
            // gets compiled down to a direct call with no branching
            if (typeof(T) == typeof(sbyte))
                Append(*(sbyte*)ptr, format);
            else if (typeof(T) == typeof(byte))
                Append(*(byte*)ptr, format);
            else if (typeof(T) == typeof(short))
                Append(*(short*)ptr, format);
            else if (typeof(T) == typeof(ushort))
                Append(*(ushort*)ptr, format);
            else if (typeof(T) == typeof(int))
                Append(*(int*)ptr, format);
            else if (typeof(T) == typeof(uint))
                Append(*(uint*)ptr, format);
            else if (typeof(T) == typeof(long))
                Append(*(long*)ptr, format);
            else if (typeof(T) == typeof(ulong))
                Append(*(ulong*)ptr, format);
            else if (typeof(T) == typeof(float))
                Append(*(float*)ptr, format);
            else if (typeof(T) == typeof(double))
                Append(*(double*)ptr, format);
            else if (typeof(T) == typeof(decimal))
                Append(*(decimal*)ptr, format);
            else if (typeof(T) == typeof(bool))
                Append(*(bool*)ptr, format);
            else if (typeof(T) == typeof(char))
                Append(*(char*)ptr, format);
            else if (typeof(T) == typeof(string)) {
                var placeholder = default(T);
                var tr = __makeref(placeholder);
                *(IntPtr*)&tr = ptr;
                Append(__refvalue(tr, string), format);
            }
            else {
                // otherwise, we have an unknown type; extract it from the pointer
                var placeholder = default(T);
                var tr = __makeref(placeholder);
                *(IntPtr*)&tr = ptr;
                var value = __refvalue(tr, T);

                // first, check to see if it's a value type implementing IStringFormattable
                var forward = ValueHelper<T>.Forward;
                if (forward != null)
                    forward(this, value, format);
                else {
                    // Only two cases left; reference type implementing IStringFormattable,
                    // or some unknown type that doesn't implement the interface at all.
                    // We could handle the latter case by calling ToString() on it and paying the
                    // allocation, but presumably if the user is using us instead of the built-in
                    // formatting utilities they would rather be notified of this case, so
                    // we'll let the cast throw.
                    ((IStringFormattable)value).Format(this, format);
                }
            }
        }

        static int ParseNum (ref char* currRef, char* end, int maxValue) {
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

        static char SkipWhitespace (ref char* currRef, char* end) {
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

    // The point of this class is to allow us to generate a direct call to a known
    // method on an unknown, unconstrained generic value type. Normally this would
    // be impossible; you'd have to cast the generic argument and introduce boxing.
    // Instead we pay a one-time startup cost to create a delegate that will forward
    // the parameter to the appropriate method in a strongly typed fashion.
    static class ValueHelper<T> {
        public static readonly Action<StringFormatter, T, StringView> Forward = Prepare();

        static Action<StringFormatter, T, StringView> Prepare () {
            // we only use this class for value types that also implement IStringFormattable
            var type = typeof(T);
            if (!type.IsValueType || !typeof(IStringFormattable).IsAssignableFrom(type))
                return null;

            var result = typeof(ValueHelper<T>)
                .GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(type)
                .Invoke(null, null);
            return (Action<StringFormatter, T, StringView>)result;
        }

        static Action<StringFormatter, U, StringView> Assign<U>() where U : IStringFormattable {
            return (f, u, v) => u.Format(f, v);
        }
    }

    public interface IArgSet {
        int Count { get; }
        void Format (StringFormatter formatter, int index, StringView format);
    }

    public interface IStringFormattable {
        void Format (StringFormatter formatter, StringView format);
    }
}