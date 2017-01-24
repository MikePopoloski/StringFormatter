using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Text.Formatting {
    /// <summary>
    /// Specifies an interface for types that act as a set of formatting arguments.
    /// </summary>
    public interface IArgSet {
        /// <summary>
        /// The number of arguments in the set.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Format one of the arguments in the set into the given string buffer.
        /// </summary>
        /// <param name="buffer">The buffer to which to append the argument.</param>
        /// <param name="index">The index of the argument to format.</param>
        /// <param name="format">A specifier indicating how the argument should be formatted.</param>
        void Format (StringBuffer buffer, int index, StringView format);
    }

    /// <summary>
    /// Defines an interface for types that can be formatted into a string buffer.
    /// </summary>
    public interface IStringFormattable {
        /// <summary>
        /// Format the current instance into the given string buffer.
        /// </summary>
        /// <param name="buffer">The buffer to which to append.</param>
        /// <param name="format">A specifier indicating how the argument should be formatted.</param>
        void Format (StringBuffer buffer, StringView format);
    }

    /// <summary>
    /// A low-allocation version of the built-in <see cref="StringBuilder"/> type.
    /// </summary>
    public unsafe sealed partial class StringBuffer {
        CachedCulture culture;
        char[] buffer;
        int currentCount;

        /// <summary>
        /// The number of characters in the buffer.
        /// </summary>
        public int Count {
            get { return currentCount; }
        }

        /// <summary>
        /// The culture used to format string data.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class.
        /// </summary>
        public StringBuffer ()
            : this(DefaultCapacity) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class.
        /// </summary>
        /// <param name="capacity">The initial size of the string buffer.</param>
        public StringBuffer (int capacity) {
            buffer = new char[capacity];
            culture = CachedCurrentCulture;
        }

        /// <summary>
        /// Sets a custom formatter to use when converting instances of a given type to a string.
        /// </summary>
        /// <typeparam name="T">The type for which to set the formatter.</typeparam>
        /// <param name="formatter">A delegate that will be called to format instances of the specified type.</param>
        public static void SetCustomFormatter<T>(Action<StringBuffer, T, StringView> formatter) {
            ValueHelper<T>.Formatter = formatter;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear () {
            currentCount = 0;
        }

        /// <summary>
        /// Copies the contents of the buffer to the given array.
        /// </summary>
        /// <param name="sourceIndex">The index within the buffer to begin copying.</param>
        /// <param name="destination">The destination array.</param>
        /// <param name="destinationIndex">The index within the destination array to which to begin copying.</param>
        /// <param name="count">The number of characters to copy.</param>
        public void CopyTo (int sourceIndex, char[] destination, int destinationIndex, int count) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destinationIndex + count > destination.Length || destinationIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));

            fixed (char* destPtr = &destination[destinationIndex])
                CopyTo(destPtr, sourceIndex, count);
        }

        /// <summary>
        /// Copies the contents of the buffer to the given array.
        /// </summary>
        /// <param name="dest">A pointer to the destination array.</param>
        /// <param name="sourceIndex">The index within the buffer to begin copying.</param>
        /// <param name="count">The number of characters to copy.</param>
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

        /// <summary>
        /// Copies the contents of the buffer to the given byte array.
        /// </summary>
        /// <param name="dest">A pointer to the destination byte array.</param>
        /// <param name="sourceIndex">The index within the buffer to begin copying.</param>
        /// <param name="count">The number of characters to copy.</param>
        /// <param name="encoding">The encoding to use to convert characters to bytes.</param>
        /// <returns>The number of bytes written to the destination.</returns>
        public int CopyTo (byte* dest, int sourceIndex, int count, Encoding encoding) {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (sourceIndex + count > currentCount || sourceIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            fixed (char* s = buffer)
                return encoding.GetBytes(s, count, dest, count);
        }

        /// <summary>
        /// Converts the buffer to a string instance.
        /// </summary>
        /// <returns>A new string representing the characters currently in the buffer.</returns>
        public override string ToString () {
            return new string(buffer, 0, currentCount);
        }

        /// <summary>
        /// Appends a character to the current buffer.
        /// </summary>
        /// <param name="c">The character to append.</param>
        public void Append (char c) {
            Append(c, 1);
        }

        /// <summary>
        /// Appends a character to the current buffer several times.
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <param name="count">The number of times to append the character.</param>
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

        /// <summary>
        /// Appends the specified string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        public void Append (string value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Append(value, 0, value.Length);
        }

        /// <summary>
        /// Appends a string subset to the current buffer.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <param name="startIndex">The starting index within the string to begin reading characters.</param>
        /// <param name="count">The number of characters to append.</param>
        public void Append (string value, int startIndex, int count) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex < 0 || startIndex + count > value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            fixed (char* s = value)
                Append(s + startIndex, count);
        }

        /// <summary>
        /// Appends an array of characters to the current buffer.
        /// </summary>
        /// <param name="values">The characters to append.</param>
        /// <param name="startIndex">The starting index within the array to begin reading characters.</param>
        /// <param name="count">The number of characters to append.</param>
        public void Append (char[] values, int startIndex, int count) {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (startIndex < 0 || startIndex + count > values.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            fixed (char* s = &values[startIndex])
                Append(s, count);
        }

        /// <summary>
        /// Appends an array of characters to the current buffer.
        /// </summary>
        /// <param name="str">A pointer to the array of characters to append.</param>
        /// <param name="count">The number of characters to append.</param>
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

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        public void Append (bool value) {
            if (value)
                Append(TrueLiteral);
            else
                Append(FalseLiteral);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (sbyte value, StringView format) {
            Numeric.FormatSByte(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (byte value, StringView format) {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (short value, StringView format) {
            Numeric.FormatInt16(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (ushort value, StringView format) {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (int value, StringView format) {
            Numeric.FormatInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (uint value, StringView format) {
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (long value, StringView format) {
            Numeric.FormatInt64(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (ulong value, StringView format) {
            Numeric.FormatUInt64(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (float value, StringView format) {
            Numeric.FormatSingle(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (double value, StringView format) {
            Numeric.FormatDouble(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append (decimal value, StringView format) {
            Numeric.FormatDecimal(this, (uint*)&value, format, culture);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <typeparam name="T">The type of argument set being formatted.</typeparam>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">The set of args to insert into the format string.</param>
        public void AppendArgSet<T>(string format, ref T args) where T : IArgSet {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            fixed (char* formatPtr = format)
            {
                var curr = formatPtr;
                var end = curr + format.Length;
                var segmentsLeft = false;
                var prevArgIndex = 0;
                do {
                    CheckCapacity((int)(end - curr));
                    fixed (char* bufferPtr = &buffer[currentCount])
                        segmentsLeft = AppendSegment(ref curr, end, bufferPtr, ref prevArgIndex, ref args);
                }
                while (segmentsLeft);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckCapacity (int count) {
            if (currentCount + count > buffer.Length)
                Array.Resize(ref buffer, buffer.Length * 2);
        }

        bool AppendSegment<T>(ref char* currRef, char* end, char* dest, ref int prevArgIndex, ref T args) where T : IArgSet {
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

            int index;
            if (*curr == '}')
                index = prevArgIndex;
            else
                index = ParseNum(ref curr, end, MaxArgs);
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

            prevArgIndex = index + 1;
            currRef = curr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AppendGeneric<T>(T value, StringView format) {
            // this looks gross, but T is known at JIT-time so this call tree
            // gets compiled down to a direct call with no branching
            if (typeof(T) == typeof(sbyte))
                Append(*(sbyte*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(byte))
                Append(*(byte*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(short))
                Append(*(short*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(ushort))
                Append(*(ushort*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(int))
                Append(*(int*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(uint))
                Append(*(uint*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(long))
                Append(*(long*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(ulong))
                Append(*(ulong*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(float))
                Append(*(float*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(double))
                Append(*(double*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(decimal))
                Append(*(decimal*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(bool))
                Append(*(bool*)Unsafe.AsPointer(ref value));
            else if (typeof(T) == typeof(char))
                Append(*(char*)Unsafe.AsPointer(ref value), format);
            else if (typeof(T) == typeof(string))
                Append(Unsafe.As<string>(value));
            else {
                // first, check to see if it's a value type implementing IStringFormattable
                var formatter = ValueHelper<T>.Formatter;
                if (formatter != null)
                    formatter(this, value, format);
                else {
                    // We could handle this case by calling ToString() on the object and paying the
                    // allocation, but presumably if the user is using us instead of the built-in
                    // formatting utilities they would rather be notified of this case, so we'll throw.
                    throw new InvalidOperationException(string.Format(SR.TypeNotFormattable, typeof(T)));
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
                if (!typeof(IStringFormattable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    return null;

                var result = typeof(ValueHelper<T>)
                    .GetTypeInfo()
                    .GetDeclaredMethod("Assign")
                    .MakeGenericMethod(type)
                    .Invoke(null, null);
                return (Action<StringBuffer, T, StringView>)result;
            }

            public static Action<StringBuffer, U, StringView> Assign<U>() where U : IStringFormattable {
                return (f, u, v) => u.Format(f, v);
            }
        }
    }
}