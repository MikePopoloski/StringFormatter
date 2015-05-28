using System;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Text.Formatting {
    public interface IArgSet {
        int Count { get; }
        void Format (StringBuffer formatter, int index, StringView format);
    }

    public interface IStringFormattable {
        void Format (StringBuffer formatter, StringView format);
    }

    public unsafe sealed partial class StringBuffer {
        CachedCulture culture;
        char[] buffer;
        int currentCount;

        public int Count {
            get { return currentCount; }
        }

        public CultureInfo Culture {
            get { return culture.Culture; }
            set {
                if (culture.Culture == value)
                    return;

                if (value == CultureInfo.InvariantCulture)
                    culture = CachedInvariantCulture;
                else if (value == CachedCurrentCulture.Culture)
                    culture = CachedCurrentCulture;
                else
                    culture = new CachedCulture(value);
            }
        }

        public StringBuffer ()
            : this(DefaultCapacity) {
        }

        public StringBuffer (int capacity) {
            buffer = new char[capacity];
            culture = CachedCurrentCulture;
        }

        public static void SetCustomFormatter<T>(Action<StringBuffer, T, StringView> formatter) {
            ValueHelper<T>.Formatter = formatter;
        }

        public void Clear () {
            currentCount = 0;
        }

        public void CopyTo (int sourceIndex, char[] destination, int destinationIndex, int count) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destinationIndex + count > destination.Length || destinationIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));

            fixed (char* destPtr = &destination[destinationIndex])
                CopyTo(destPtr, sourceIndex, count);
        }

        public void CopyTo (char* dest, int sourceIndex, int count) {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (sourceIndex + count > currentCount || sourceIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));

            fixed (char* s = buffer)
            {
                var src = s + sourceIndex;
                for (int i = 0; i < count; i++)
                    *dest++ = *src++;
            }
        }

        public override string ToString () {
            return new string(buffer, 0, currentCount);
        }

        public void Append (char c) {
            Append(c, 1);
        }

        public void Append (char c, int count) {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            CheckCapacity(count);
            fixed (char* b = &buffer[currentCount])
            {
                var ptr = b;
                for (int i = 0; i < count; i++)
                    *ptr++ = c;
                currentCount += count;
            }
        }

        public void Append (string value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Append(value, 0, value.Length);
        }

        public void Append (string value, int startIndex, int count) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex < 0 || startIndex + count > value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            fixed (char* s = value)
                Append(s + startIndex, count);
        }

        public void Append (char[] values, int startIndex, int count) {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (startIndex < 0 || startIndex + count > values.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            fixed (char* s = &values[startIndex])
                Append(s, count);
        }

        public void Append (char* str, int count) {
            CheckCapacity(count);
            fixed (char* b = &buffer[currentCount])
            {
                var dest = b;
                for (int i = 0; i < count; i++)
                    *dest++ = *str++;
                currentCount += count;
            }
        }

        public void Append (bool value) {
            if (value)
                Append(TrueLiteral);
            else
                Append(FalseLiteral);
        }

        public void Append (sbyte value, StringView format) {
            Numeric.FormatSByte(this, value, format, culture);
        }

        public void Append (byte value, StringView format) {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        public void Append (short value, StringView format) {
            Numeric.FormatInt16(this, value, format, culture);
        }

        public void Append (ushort value, StringView format) {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        public void Append (int value, StringView format) {
            Numeric.FormatInt32(this, value, format, culture);
        }

        public void Append (uint value, StringView format) {
            Numeric.FormatUInt32(this, value, format, culture);
        }

        public void Append (long value, StringView format) {
            Numeric.FormatInt64(this, value, format, culture);
        }

        public void Append (ulong value, StringView format) {
            Numeric.FormatUInt64(this, value, format, culture);
        }

        public void Append (float value, StringView format) {
            Numeric.FormatSingle(this, value, format, culture);
        }

        public void Append (double value, StringView format) {
            Numeric.FormatDouble(this, value, format, culture);
        }

        public void Append (decimal value, StringView format) {
            Numeric.FormatDecimal(this, (uint*)&value, format, culture);
        }

        public void AppendArgSet<T>(string format, ref T args) where T : IArgSet {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            fixed (char* formatPtr = format)
            {
                var curr = formatPtr;
                var end = curr + format.Length;
                var segmentsLeft = false;
                do {
                    CheckCapacity((int)(end - curr));
                    fixed (char* bufferPtr = &buffer[currentCount])
                        segmentsLeft = AppendSegment(ref curr, end, bufferPtr, ref args);
                }
                while (segmentsLeft);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckCapacity (int count) {
            if (currentCount + count > buffer.Length)
                Array.Resize(ref buffer, buffer.Length * 2);
        }

        bool AppendSegment<T>(ref char* currRef, char* end, char* dest, ref T args) where T : IArgSet {
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

                *dest++ = c;
                currentCount++;
            }

            if (curr == end)
                return false;

            var index = ParseNum(ref curr, end, MaxArgs);
            if (index >= args.Count)
                throw new FormatException(string.Format(SR.ArgIndexOutOfRange, index));

            // check for a spacing specifier
            c = SkipWhitespace(ref curr, end);
            var width = 0;
            var leftJustify = false;
            var oldCount = currentCount;
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
            curr++;
            if (c == ':') {
                var specifierBuffer = stackalloc char[MaxSpecifierSize];
                var specifierEnd = specifierBuffer + MaxSpecifierSize;
                var specifierPtr = specifierBuffer;

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
                            // found the end of the specifier
                            // kick off the format job
                            var specifier = new StringView(specifierBuffer, (int)(specifierPtr - specifierBuffer));
                            args.Format(this, index, specifier);
                            break;
                        }
                    }

                    if (specifierPtr == specifierEnd)
                        ThrowError();
                    *specifierPtr++ = c;
                }
            }
            else {
                // no specifier. make sure we're at the end of the format block
                if (c != '}')
                    ThrowError();

                // format without any specifier
                args.Format(this, index, StringView.Empty);
            }

            // finish off padding, if necessary
            var padding = width - (currentCount - oldCount);
            if (padding > 0) {
                if (leftJustify)
                    Append(' ', padding);
                else {
                    // copy the recently placed chars up in memory to make room for padding
                    CheckCapacity(padding);
                    for (int i = currentCount - 1; i >= oldCount; i--)
                        buffer[i + padding] = buffer[i];

                    // fill in padding
                    for (int i = 0; i < padding; i++)
                        buffer[i + oldCount] = ' ';
                    currentCount += padding;
                }
            }

            currRef = curr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                Append(*(bool*)ptr);
            else if (typeof(T) == typeof(char))
                Append(*(char*)ptr, format);
            else if (typeof(T) == typeof(string)) {
                var placeholder = default(T);
                var tr = __makeref(placeholder);
                *(IntPtr*)&tr = ptr;
                Append(__refvalue(tr, string));
            }
            else {
                // otherwise, we have an unknown type; extract it from the pointer
                var placeholder = default(T);
                var tr = __makeref(placeholder);
                *(IntPtr*)&tr = ptr;
                var value = __refvalue(tr, T);

                // first, check to see if it's a value type implementing IStringFormattable
                var formatter = ValueHelper<T>.Formatter;
                if (formatter != null)
                    formatter(this, value, format);
                else {
                    // Only two cases left; reference type implementing IStringFormattable,
                    // or some unknown type that doesn't implement the interface at all.
                    // We could handle the latter case by calling ToString() on it and paying the
                    // allocation, but presumably if the user is using us instead of the built-in
                    // formatting utilities they would rather be notified of this case, so
                    // we'll let the cast throw.
                    var formattable = value as IStringFormattable;
                    if (formattable == null)
                        throw new InvalidOperationException(string.Format(SR.TypeNotFormattable, typeof(T)));
                    formattable.Format(this, format);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char SkipWhitespace (ref char* currRef, char* end) {
            char* curr = currRef;
            while (curr < end && *curr == ' ') curr++;

            if (curr == end)
                ThrowError();

            currRef = curr;
            return *curr;
        }

        static void ThrowError () {
            throw new FormatException(SR.InvalidFormatString);
        }

        static StringBuffer Acquire (int capacity) {
            if (capacity <= MaxCachedSize) {
                var buffer = CachedInstance;
                if (buffer != null) {
                    CachedInstance = null;
                    buffer.Clear();
                    buffer.CheckCapacity(capacity);
                    return buffer;
                }
            }

            return new StringBuffer(capacity);
        }

        static void Release (StringBuffer buffer) {
            if (buffer.buffer.Length <= MaxCachedSize)
                CachedInstance = buffer;
        }

        [ThreadStatic]
        static StringBuffer CachedInstance;

        static readonly CachedCulture CachedInvariantCulture = new CachedCulture(CultureInfo.InvariantCulture);
        static readonly CachedCulture CachedCurrentCulture = new CachedCulture(CultureInfo.CurrentCulture);

        const int DefaultCapacity = 32;
        const int MaxCachedSize = 360;  // same as BCL's StringBuilderCache
        const int MaxArgs = 256;
        const int MaxSpacing = 1000000;
        const int MaxSpecifierSize = 32;

        const string TrueLiteral = "True";
        const string FalseLiteral = "False";

        // The point of this class is to allow us to generate a direct call to a known
        // method on an unknown, unconstrained generic value type. Normally this would
        // be impossible; you'd have to cast the generic argument and introduce boxing.
        // Instead we pay a one-time startup cost to create a delegate that will forward
        // the parameter to the appropriate method in a strongly typed fashion.
        static class ValueHelper<T> {
            public static Action<StringBuffer, T, StringView> Formatter = Prepare();

            static Action<StringBuffer, T, StringView> Prepare () {
                // we only use this class for value types that also implement IStringFormattable
                var type = typeof(T);
                if (!type.IsValueType || !typeof(IStringFormattable).IsAssignableFrom(type))
                    return null;

                var result = typeof(ValueHelper<T>)
                    .GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(type)
                    .Invoke(null, null);
                return (Action<StringBuffer, T, StringView>)result;
            }

            static Action<StringBuffer, U, StringView> Assign<U>() where U : IStringFormattable {
                return (f, u, v) => u.Format(f, v);
            }
        }
    }
}