using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    // Most of the implementation of this file was ported from the native versions built into the CLR
    // See: https://github.com/dotnet/coreclr/blob/838807429a0828a839958e3b7d392d65886c8f2e/src/classlibnative/bcltype/number.cpp
    // Also see: https://github.com/dotnet/coreclr/blob/02084af832c2900cf6eac2a168c41f261409be97/src/mscorlib/src/System/Number.cs
    // Standard numeric format string reference: https://msdn.microsoft.com/en-us/library/dwhawy9k%28v=vs.110%29.aspx

    unsafe static partial class Numeric {
        public static void FormatSByte (StringBuffer formatter, sbyte value, StringView specifier, CachedCulture culture) {
            if (value < 0 && !specifier.IsEmpty) {
                // if we're negative and doing a hex format, mask out the bits for the conversion
                char c = specifier.Data[0];
                if (c == 'X' || c == 'x') {
                    FormatUInt32(formatter, (uint)(value & 0xFF), specifier, culture);
                    return;
                }
            }

            FormatInt32(formatter, value, specifier, culture);
        }

        public static void FormatInt16 (StringBuffer formatter, short value, StringView specifier, CachedCulture culture) {
            if (value < 0 && !specifier.IsEmpty) {
                // if we're negative and doing a hex format, mask out the bits for the conversion
                char c = specifier.Data[0];
                if (c == 'X' || c == 'x') {
                    FormatUInt32(formatter, (uint)(value & 0xFFFF), specifier, culture);
                    return;
                }
            }

            FormatInt32(formatter, value, specifier, culture);
        }

        public static void FormatInt32 (StringBuffer formatter, int value, StringView specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 0)
                        goto default;
                    else
                        goto case 'D';

                case 'D':
                    Int32ToDecStr(formatter, value, digits, culture.NegativeSign);
                    break;

                case 'X':
                    // fmt-('X'-'A'+1) gives us the base hex character in either
                    // uppercase or lowercase, depending on the casing of fmt
                    Int32ToHexStr(formatter, (uint)value, fmt - ('X' - 'A' + 10), digits);
                    break;

                default:
                    var number = new Number();
                    var buffer = stackalloc char[MaxNumberDigits + 1];
                    number.Digits = buffer;
                    Int32ToNumber(value, ref number);
                    if (fmt != 0)
                        NumberToString(formatter, ref number, fmt, digits, culture);
                    else
                        NumberToCustomFormatString(formatter, ref number, specifier, culture);
                    break;
            }
        }

        public static void FormatUInt32 (StringBuffer formatter, uint value, StringView specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 0)
                        goto default;
                    else
                        goto case 'D';

                case 'D':
                    UInt32ToDecStr(formatter, value, digits);
                    break;

                case 'X':
                    // fmt-('X'-'A'+1) gives us the base hex character in either
                    // uppercase or lowercase, depending on the casing of fmt
                    Int32ToHexStr(formatter, value, fmt - ('X' - 'A' + 10), digits);
                    break;

                default:
                    var number = new Number();
                    var buffer = stackalloc char[MaxNumberDigits + 1];
                    number.Digits = buffer;
                    UInt32ToNumber(value, ref number);
                    if (fmt != 0)
                        NumberToString(formatter, ref number, fmt, digits, culture);
                    else
                        NumberToCustomFormatString(formatter, ref number, specifier, culture);
                    break;
            }
        }

        public static void FormatInt64 (StringBuffer formatter, long value, StringView specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 0)
                        goto default;
                    else
                        goto case 'D';

                case 'D':
                    Int64ToDecStr(formatter, value, digits, culture.NegativeSign);
                    break;

                case 'X':
                    // fmt-('X'-'A'+1) gives us the base hex character in either
                    // uppercase or lowercase, depending on the casing of fmt
                    Int64ToHexStr(formatter, (ulong)value, fmt - ('X' - 'A' + 10), digits);
                    break;

                default:
                    var number = new Number();
                    var buffer = stackalloc char[MaxNumberDigits + 1];
                    number.Digits = buffer;
                    Int64ToNumber(value, ref number);
                    if (fmt != 0)
                        NumberToString(formatter, ref number, fmt, digits, culture);
                    else
                        NumberToCustomFormatString(formatter, ref number, specifier, culture);
                    break;
            }
        }

        public static void FormatUInt64 (StringBuffer formatter, ulong value, StringView specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 0)
                        goto default;
                    else
                        goto case 'D';

                case 'D':
                    UInt64ToDecStr(formatter, value, digits);
                    break;

                case 'X':
                    // fmt-('X'-'A'+1) gives us the base hex character in either
                    // uppercase or lowercase, depending on the casing of fmt
                    Int64ToHexStr(formatter, value, fmt - ('X' - 'A' + 10), digits);
                    break;

                default:
                    var number = new Number();
                    var buffer = stackalloc char[MaxNumberDigits + 1];
                    number.Digits = buffer;
                    UInt64ToNumber(value, ref number);
                    if (fmt != 0)
                        NumberToString(formatter, ref number, fmt, digits, culture);
                    else
                        NumberToCustomFormatString(formatter, ref number, specifier, culture);
                    break;
            }
        }

        public static void FormatSingle (StringBuffer formatter, float value, StringView specifier, CachedCulture culture) {
            int digits;
            int precision = FloatPrecision;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 7)
                        precision = 9;
                    break;

                case 'E':
                    if (digits > 6)
                        precision = 9;
                    break;
            }

            var number = new Number();
            var buffer = stackalloc char[MaxFloatingDigits + 1];
            number.Digits = buffer;
            DoubleToNumber(value, precision, ref number);

            if (number.Scale == ScaleNaN) {
                formatter.Append(culture.NaN);
                return;
            }

            if (number.Scale == ScaleInf) {
                if (number.Sign > 0)
                    formatter.Append(culture.NegativeInfinity);
                else
                    formatter.Append(culture.PositiveInfinity);
                return;
            }

            if (fmt != 0)
                NumberToString(formatter, ref number, fmt, digits, culture);
            else
                NumberToCustomFormatString(formatter, ref number, specifier, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatDouble (StringBuffer formatter, double value, StringView specifier, CachedCulture culture) {
            int digits;
            int precision = DoublePrecision;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (fmt & 0xFFDF) {
                case 'G':
                    if (digits > 15)
                        precision = 17;
                    break;

                case 'E':
                    if (digits > 14)
                        precision = 17;
                    break;
            }

            var number = new Number();
            var buffer = stackalloc char[MaxFloatingDigits + 1];
            number.Digits = buffer;
            DoubleToNumber(value, precision, ref number);

            if (number.Scale == ScaleNaN) {
                formatter.Append(culture.NaN);
                return;
            }

            if (number.Scale == ScaleInf) {
                if (number.Sign > 0)
                    formatter.Append(culture.NegativeInfinity);
                else
                    formatter.Append(culture.PositiveInfinity);
                return;
            }

            if (fmt != 0)
                NumberToString(formatter, ref number, fmt, digits, culture);
            else
                NumberToCustomFormatString(formatter, ref number, specifier, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatDecimal (StringBuffer formatter, uint* value, StringView specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);

            var number = new Number();
            var buffer = stackalloc char[MaxNumberDigits + 1];
            number.Digits = buffer;
            DecimalToNumber(value, ref number);
            if (fmt != 0)
                NumberToString(formatter, ref number, fmt, digits, culture, isDecimal: true);
            else
                NumberToCustomFormatString(formatter, ref number, specifier, culture);
        }

        static void NumberToString (StringBuffer formatter, ref Number number, char format, int maxDigits, CachedCulture culture, bool isDecimal = false) {
            // ANDing with 0xFFDF has the effect of uppercasing the character
            switch (format & 0xFFDF) {
                case 'C':
                    {
                        var cultureData = culture.CurrencyData;
                        var bufferSize = cultureData.GetBufferSize(ref maxDigits, number.Scale);
                        RoundNumber(ref number, number.Scale + maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = FormatCurrency(
                            buffer,
                            ref number,
                            maxDigits,
                            cultureData,
                            number.Sign > 0 ? culture.CurrencyNegativePattern : culture.CurrencyPositivePattern,
                            culture.CurrencySymbol
                        );

                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                case 'F':
                    {
                        var cultureData = culture.FixedData;
                        var bufferSize = cultureData.GetBufferSize(ref maxDigits, number.Scale);
                        RoundNumber(ref number, number.Scale + maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = buffer;
                        if (number.Sign > 0)
                            AppendString(&ptr, cultureData.NegativeSign);

                        ptr = FormatFixed(ptr, ref number, maxDigits, cultureData);
                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                case 'N':
                    {
                        var cultureData = culture.NumberData;
                        var bufferSize = cultureData.GetBufferSize(ref maxDigits, number.Scale);
                        RoundNumber(ref number, number.Scale + maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = FormatNumber(
                            buffer,
                            ref number,
                            maxDigits,
                            number.Sign > 0 ? culture.NumberNegativePattern : culture.NumberPositivePattern,
                            cultureData
                        );

                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                case 'E':
                    {
                        var cultureData = culture.ScientificData;
                        var bufferSize = cultureData.GetBufferSize(ref maxDigits, number.Scale);
                        maxDigits++;

                        RoundNumber(ref number, maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = buffer;
                        if (number.Sign > 0)
                            AppendString(&ptr, cultureData.NegativeSign);

                        ptr = FormatScientific(
                            ptr,
                            ref number,
                            maxDigits,
                            format, // TODO: fix casing
                            cultureData.DecimalSeparator,
                            culture.PositiveSign,
                            culture.NegativeSign
                        );

                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                case 'P':
                    {
                        number.Scale += 2;
                        var cultureData = culture.PercentData;
                        var bufferSize = cultureData.GetBufferSize(ref maxDigits, number.Scale);
                        RoundNumber(ref number, number.Scale + maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = FormatPercent(
                            buffer,
                            ref number,
                            maxDigits,
                            cultureData,
                            number.Sign > 0 ? culture.PercentNegativePattern : culture.PercentPositivePattern,
                            culture.PercentSymbol
                        );

                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                case 'G':
                    {
                        var enableRounding = true;
                        if (maxDigits < 1) {
                            if (isDecimal && maxDigits == -1) {
                                // if we're formatting a decimal, default to 29 digits precision
                                // only for G formatting without a precision specifier
                                maxDigits = DecimalPrecision;
                                enableRounding = false;
                            }
                            else
                                maxDigits = number.Precision;
                        }

                        var bufferSize = maxDigits + culture.DecimalBufferSize;
                        var buffer = stackalloc char[bufferSize];
                        var ptr = buffer;

                        // round for G formatting only if a precision is given
                        // we need to handle the minus zero case also
                        if (enableRounding)
                            RoundNumber(ref number, maxDigits);
                        else if (isDecimal && number.Digits[0] == 0)
                            number.Sign = 0;

                        if (number.Sign > 0)
                            AppendString(&ptr, culture.NegativeSign);

                        ptr = FormatGeneral(
                            ptr,
                            ref number,
                            maxDigits,
                            (char)(format - ('G' - 'E')),
                            culture.NumberData.DecimalSeparator,
                            culture.PositiveSign,
                            culture.NegativeSign,
                            !enableRounding
                        );

                        formatter.Append(buffer, (int)(ptr - buffer));
                        break;
                    }

                default:
                    throw new FormatException(string.Format(SR.UnknownFormatSpecifier, format));
            }
        }

        static char* FormatCurrency (char* buffer, ref Number number, int maxDigits, NumberFormatData data, string currencyFormat, string currencySymbol) {
            for (int i = 0; i < currencyFormat.Length; i++) {
                char c = currencyFormat[i];
                switch (c) {
                    case '#': buffer = FormatFixed(buffer, ref number, maxDigits, data); break;
                    case '-': AppendString(&buffer, data.NegativeSign); break;
                    case '$': AppendString(&buffer, currencySymbol); break;
                    default: *buffer++ = c; break;
                }
            }

            return buffer;
        }

        static char* FormatNumber (char* buffer, ref Number number, int maxDigits, string format, NumberFormatData data) {
            for (int i = 0; i < format.Length; i++) {
                char c = format[i];
                switch (c) {
                    case '#': buffer = FormatFixed(buffer, ref number, maxDigits, data); break;
                    case '-': AppendString(&buffer, data.NegativeSign); break;
                    default: *buffer++ = c; break;
                }
            }

            return buffer;
        }

        static char* FormatPercent (char* buffer, ref Number number, int maxDigits, NumberFormatData data, string format, string percentSymbol) {
            for (int i = 0; i < format.Length; i++) {
                char c = format[i];
                switch (c) {
                    case '#': buffer = FormatFixed(buffer, ref number, maxDigits, data); break;
                    case '-': AppendString(&buffer, data.NegativeSign); break;
                    case '%': AppendString(&buffer, percentSymbol); break;
                    default: *buffer++ = c; break;
                }
            }

            return buffer;
        }

        static char* FormatGeneral (
            char* buffer, ref Number number, int maxDigits, char expChar,
            string decimalSeparator, string positiveSign, string negativeSign,
            bool suppressScientific) {

            var digitPos = number.Scale;
            var scientific = false;
            if (!suppressScientific) {
                if (digitPos > maxDigits || digitPos < -3) {
                    digitPos = 1;
                    scientific = true;
                }
            }

            var digits = number.Digits;
            if (digitPos <= 0)
                *buffer++ = '0';
            else {
                do {
                    *buffer++ = *digits != 0 ? *digits++ : '0';
                } while (--digitPos > 0);
            }

            if (*digits != 0 || digitPos < 0) {
                AppendString(&buffer, decimalSeparator);
                while (digitPos < 0) {
                    *buffer++ = '0';
                    digitPos++;
                }

                while (*digits != 0)
                    *buffer++ = *digits++;
            }

            if (scientific)
                buffer = FormatExponent(buffer, number.Scale - 1, expChar, positiveSign, negativeSign, 2);

            return buffer;
        }

        static char* FormatScientific (
            char* buffer, ref Number number, int maxDigits, char expChar,
            string decimalSeparator, string positiveSign, string negativeSign) {

            var digits = number.Digits;
            *buffer++ = *digits != 0 ? *digits++ : '0';
            if (maxDigits != 1)
                AppendString(&buffer, decimalSeparator);

            while (--maxDigits > 0)
                *buffer++ = *digits != 0 ? *digits++ : '0';

            int e = number.Digits[0] == 0 ? 0 : number.Scale - 1;
            return FormatExponent(buffer, e, expChar, positiveSign, negativeSign, 3);
        }

        static char* FormatExponent (char* buffer, int value, char expChar, string positiveSign, string negativeSign, int minDigits) {
            *buffer++ = expChar;
            if (value < 0) {
                AppendString(&buffer, negativeSign);
                value = -value;
            }
            else if (positiveSign != null)
                AppendString(&buffer, positiveSign);

            var digits = stackalloc char[11];
            var ptr = Int32ToDecChars(digits + 10, (uint)value, minDigits);
            var len = (int)(digits + 10 - ptr);
            while (--len >= 0)
                *buffer++ = *ptr++;

            return buffer;
        }

        static char* FormatFixed (char* buffer, ref Number number, int maxDigits, NumberFormatData data) {
            var groups = data.GroupSizes;
            var digits = number.Digits;
            var digitPos = number.Scale;
            if (digitPos <= 0)
                *buffer++ = '0';
            else if (groups != null) {
                var groupIndex = 0;
                var groupSizeCount = groups[0];
                var groupSizeLen = groups.Length;
                var newBufferSize = digitPos;
                var groupSeparatorLen = data.GroupSeparator.Length;
                var groupSize = 0;

                // figure out the size of the result
                if (groupSizeLen != 0) {
                    while (digitPos > groupSizeCount) {
                        groupSize = groups[groupIndex];
                        if (groupSize == 0)
                            break;

                        newBufferSize += groupSeparatorLen;
                        if (groupIndex < groupSizeLen - 1)
                            groupIndex++;

                        groupSizeCount += groups[groupIndex];
                        if (groupSizeCount < 0 || newBufferSize < 0)
                            throw new ArgumentOutOfRangeException(SR.InvalidGroupSizes);
                    }

                    if (groupSizeCount == 0)
                        groupSize = 0;
                    else
                        groupSize = groups[0];
                }

                groupIndex = 0;
                var digitCount = 0;
                var digitLength = StrLen(digits);
                var digitStart = digitPos < digitLength ? digitPos : digitLength;
                var ptr = buffer + newBufferSize - 1;

                for (int i = digitPos - 1; i >= 0; i--) {
                    *(ptr--) = i < digitStart ? digits[i] : '0';

                    // check if we need to add a group separator
                    if (groupSize > 0) {
                        digitCount++;
                        if (digitCount == groupSize && i != 0) {
                            for (int j = groupSeparatorLen - 1; j >= 0; j--)
                                *(ptr--) = data.GroupSeparator[j];

                            if (groupIndex < groupSizeLen - 1) {
                                groupIndex++;
                                groupSize = groups[groupIndex];
                            }
                            digitCount = 0;
                        }
                    }
                }

                buffer += newBufferSize;
                digits += digitStart;
            }
            else {
                do {
                    *buffer++ = *digits != 0 ? *digits++ : '0';
                }
                while (--digitPos > 0);
            }

            if (maxDigits > 0) {
                AppendString(&buffer, data.DecimalSeparator);
                while (digitPos < 0 && maxDigits > 0) {
                    *buffer++ = '0';
                    digitPos++;
                    maxDigits--;
                }

                while (maxDigits > 0) {
                    *buffer++ = *digits != 0 ? *digits++ : '0';
                    maxDigits--;
                }
            }

            return buffer;
        }

        static void Int32ToDecStr (StringBuffer formatter, int value, int digits, string negativeSign) {
            if (digits < 1)
                digits = 1;

            var maxDigits = digits > 15 ? digits : 15;
            var bufferLength = maxDigits > 100 ? maxDigits : 100;
            var negativeLength = 0;

            if (value < 0) {
                negativeLength = negativeSign.Length;
                if (negativeLength > bufferLength - maxDigits)
                    bufferLength = negativeLength + maxDigits;
            }

            var buffer = stackalloc char[bufferLength];
            var p = Int32ToDecChars(buffer + bufferLength, value >= 0 ? (uint)value : (uint)-value, digits);
            if (value < 0) {
                // add the negative sign
                for (int i = negativeLength - 1; i >= 0; i--)
                    *(--p) = negativeSign[i];
            }

            formatter.Append(p, (int)(buffer + bufferLength - p));
        }

        static void UInt32ToDecStr (StringBuffer formatter, uint value, int digits) {
            var buffer = stackalloc char[100];
            if (digits < 1)
                digits = 1;

            var p = Int32ToDecChars(buffer + 100, value, digits);
            formatter.Append(p, (int)(buffer + 100 - p));
        }

        static void Int32ToHexStr (StringBuffer formatter, uint value, int hexBase, int digits) {
            var buffer = stackalloc char[100];
            if (digits < 1)
                digits = 1;

            var p = Int32ToHexChars(buffer + 100, value, hexBase, digits);
            formatter.Append(p, (int)(buffer + 100 - p));
        }

        static void Int64ToDecStr (StringBuffer formatter, long value, int digits, string negativeSign) {
            if (digits < 1)
                digits = 1;

            var sign = (int)High32((ulong)value);
            var maxDigits = digits > 20 ? digits : 20;
            var bufferLength = maxDigits > 100 ? maxDigits : 100;

            if (sign < 0) {
                value = -value;
                var negativeLength = negativeSign.Length;
                if (negativeLength > bufferLength - maxDigits)
                    bufferLength = negativeLength + maxDigits;
            }

            var buffer = stackalloc char[bufferLength];
            var p = buffer + bufferLength;
            var uv = (ulong)value;
            while (High32(uv) != 0) {
                p = Int32ToDecChars(p, Int64DivMod(ref uv), 9);
                digits -= 9;
            }

            p = Int32ToDecChars(p, Low32(uv), digits);
            if (sign < 0) {
                // add the negative sign
                for (int i = negativeSign.Length - 1; i >= 0; i--)
                    *(--p) = negativeSign[i];
            }

            formatter.Append(p, (int)(buffer + bufferLength - p));
        }

        static void UInt64ToDecStr (StringBuffer formatter, ulong value, int digits) {
            if (digits < 1)
                digits = 1;

            var buffer = stackalloc char[100];
            var p = buffer + 100;
            while (High32(value) != 0) {
                p = Int32ToDecChars(p, Int64DivMod(ref value), 9);
                digits -= 9;
            }

            p = Int32ToDecChars(p, Low32(value), digits);
            formatter.Append(p, (int)(buffer + 100 - p));
        }

        static void Int64ToHexStr (StringBuffer formatter, ulong value, int hexBase, int digits) {
            var buffer = stackalloc char[100];
            char* ptr;
            if (High32(value) != 0) {
                Int32ToHexChars(buffer + 100, Low32(value), hexBase, 8);
                ptr = Int32ToHexChars(buffer + 100 - 8, High32(value), hexBase, digits - 8);
            }
            else {
                if (digits < 1)
                    digits = 1;
                ptr = Int32ToHexChars(buffer + 100, Low32(value), hexBase, digits);
            }

            formatter.Append(ptr, (int)(buffer + 100 - ptr));
        }

        static char* Int32ToDecChars (char* p, uint value, int digits) {
            while (value != 0) {
                *--p = (char)(value % 10 + '0');
                value /= 10;
                digits--;
            }

            while (--digits >= 0)
                *--p = '0';
            return p;
        }

        static char* Int32ToHexChars (char* p, uint value, int hexBase, int digits) {
            while (--digits >= 0 || value != 0) {
                var digit = value & 0xF;
                *--p = (char)(digit + (digit < 10 ? '0' : hexBase));
                value >>= 4;
            }
            return p;
        }

        static char ParseFormatSpecifier (StringView specifier, out int digits) {
            if (specifier.IsEmpty) {
                digits = -1;
                return 'G';
            }

            char* curr = specifier.Data;
            char first = *curr++;
            if ((first >= 'A' && first <= 'Z') || (first >= 'a' && first <= 'z')) {
                int n = -1;
                char c = *curr++;
                if (c >= '0' && c <= '9') {
                    n = c - '0';
                    c = *curr++;
                    while (c >= '0' && c <= '9') {
                        n = n * 10 + c - '0';
                        c = *curr++;
                        if (n >= 10)
                            break;
                    }
                }

                if (c == 0) {
                    digits = n;
                    return first;
                }
            }

            digits = -1;
            return (char)0;
        }

        static void Int32ToNumber (int value, ref Number number) {
            number.Precision = Int32Precision;
            if (value >= 0)
                number.Sign = 0;
            else {
                number.Sign = 1;
                value = -value;
            }

            var buffer = stackalloc char[Int32Precision + 1];
            var ptr = Int32ToDecChars(buffer + Int32Precision, (uint)value, 0);
            var len = (int)(buffer + Int32Precision - ptr);
            number.Scale = len;

            var dest = number.Digits;
            while (--len >= 0)
                *dest++ = *ptr++;
            *dest = '\0';
        }

        static void UInt32ToNumber (uint value, ref Number number) {
            number.Precision = UInt32Precision;
            number.Sign = 0;

            var buffer = stackalloc char[UInt32Precision + 1];
            var ptr = Int32ToDecChars(buffer + UInt32Precision, value, 0);
            var len = (int)(buffer + UInt32Precision - ptr);
            number.Scale = len;

            var dest = number.Digits;
            while (--len >= 0)
                *dest++ = *ptr++;
            *dest = '\0';
        }

        static void Int64ToNumber (long value, ref Number number) {
            number.Precision = Int64Precision;
            if (value >= 0)
                number.Sign = 0;
            else {
                number.Sign = 1;
                value = -value;
            }

            var buffer = stackalloc char[Int64Precision + 1];
            var ptr = buffer + Int64Precision;
            var uv = (ulong)value;
            while (High32(uv) != 0)
                ptr = Int32ToDecChars(ptr, Int64DivMod(ref uv), 9);

            ptr = Int32ToDecChars(ptr, Low32(uv), 0);
            var len = (int)(buffer + Int64Precision - ptr);
            number.Scale = len;

            var dest = number.Digits;
            while (--len >= 0)
                *dest++ = *ptr++;
            *dest = '\0';
        }

        static void UInt64ToNumber (ulong value, ref Number number) {
            number.Precision = UInt64Precision;
            number.Sign = 0;

            var buffer = stackalloc char[UInt64Precision + 1];
            var ptr = buffer + UInt64Precision;
            while (High32(value) != 0)
                ptr = Int32ToDecChars(ptr, Int64DivMod(ref value), 9);

            ptr = Int32ToDecChars(ptr, Low32(value), 0);

            var len = (int)(buffer + UInt64Precision - ptr);
            number.Scale = len;

            var dest = number.Digits;
            while (--len >= 0)
                *dest++ = *ptr++;
            *dest = '\0';
        }

        static void DoubleToNumber (double value, int precision, ref Number number) {
            number.Precision = precision;

            uint sign, exp, mantHi, mantLo;
            ExplodeDouble(value, out sign, out exp, out mantHi, out mantLo);

            if (exp == 0x7FF) {
                // special value handling (infinity and NaNs)
                number.Scale = (mantLo != 0 || mantHi != 0) ? ScaleNaN : ScaleInf;
                number.Sign = (int)sign;
                number.Digits[0] = '\0';
            }
            else {
                // convert the digits of the number to characters
                if (value < 0) {
                    number.Sign = 1;
                    value = -value;
                }

                var digits = number.Digits;
                var end = digits + MaxFloatingDigits;
                var p = end;
                var shift = 0;
                double intPart;
                double reducedInt;
                var fracPart = ModF(value, out intPart);

                if (intPart != 0) {
                    // format the integer part
                    while (intPart != 0) {
                        reducedInt = ModF(intPart / 10, out intPart);
                        *--p = (char)((int)((reducedInt + 0.03) * 10) + '0');
                        shift++;
                    }
                    while (p < end)
                        *digits++ = *p++;
                }
                else if (fracPart > 0) {
                    // normalize the fractional part
                    while ((reducedInt = fracPart * 10) < 1) {
                        fracPart = reducedInt;
                        shift--;
                    }
                }

                // concat the fractional part, padding the remainder with zeros
                p = number.Digits + precision;
                while (digits <= p && digits < end) {
                    fracPart *= 10;
                    fracPart = ModF(fracPart, out reducedInt);
                    *digits++ = (char)((int)reducedInt + '0');
                }

                // round the result if necessary
                digits = p;
                *p = (char)(*p + 5);
                while (*p > '9') {
                    *p = '0';
                    if (p > number.Digits)
                        ++*--p;
                    else {
                        *p = '1';
                        shift++;
                    }
                }

                number.Scale = shift;
                *digits = '\0';
            }
        }

        static void DecimalToNumber (uint* value, ref Number number) {
            // bit 31 of the decimal is the sign bit
            // bits 16-23 contain the scale
            number.Sign = (int)(*value >> 31);
            number.Scale = (int)((*value >> 16) & 0xFF);
            number.Precision = DecimalPrecision;

            // loop for as long as the decimal is larger than 32 bits
            var buffer = stackalloc char[DecimalPrecision + 1];
            var p = buffer + DecimalPrecision;
            var hi = *(value + 1);
            var lo = *(value + 2);
            var mid = *(value + 3);

            while ((mid | hi) != 0) {
                // keep dividing down by one billion at a time
                ulong n = hi;
                hi = (uint)(n / OneBillion);
                n = (n % OneBillion) << 32 | mid;
                mid = (uint)(n / OneBillion);
                n = (n % OneBillion) << 32 | lo;
                lo = (uint)(n / OneBillion);

                // format this portion of the number
                p = Int32ToDecChars(p, (uint)(n % OneBillion), 9);
            }

            // finish off with the low 32-bits of the decimal, if anything is left over
            p = Int32ToDecChars(p, lo, 0);

            var len = (int)(buffer + DecimalPrecision - p);
            number.Scale = len - number.Scale;

            var dest = number.Digits;
            while (--len >= 0)
                *dest++ = *p++;
            *dest = '\0';
        }

        static void RoundNumber (ref Number number, int pos) {
            var digits = number.Digits;
            int i = 0;
            while (i < pos && digits[i] != 0) i++;
            if (i == pos && digits[i] >= '5') {
                while (i > 0 && digits[i - 1] == '9') i--;
                if (i > 0)
                    digits[i - 1]++;
                else {
                    number.Scale++;
                    digits[0] = '1';
                    i = 1;
                }
            }
            else {
                while (i > 0 && digits[i - 1] == '0')
                    i--;
            }

            if (i == 0) {
                number.Scale = 0;
                number.Sign = 0;
            }

            digits[i] = '\0';
        }

        static void AppendString (char** buffer, string value) {
            fixed (char* pinnedString = value)
            {
                var length = value.Length;
                for (var src = pinnedString; src < pinnedString + length; (*buffer)++, src++)
                    **buffer = *src;
            }
        }

        static int StrLen (char* str) {
            int count = 0;
            while (*str++ != 0)
                count++;

            return count;
        }

        static uint Int64DivMod (ref ulong value) {
            var rem = (uint)(value % 1000000000);
            value /= 1000000000;
            return rem;
        }

        static double ModF (double value, out double intPart) {
            intPart = Math.Truncate(value);
            return value - intPart;
        }

        static void ExplodeDouble (double value, out uint sign, out uint exp, out uint mantHi, out uint mantLo) {
            var bits = *(ulong*)&value;
            if (BitConverter.IsLittleEndian) {
                mantLo = (uint)(bits & 0xFFFFFFFF);         // bits 0 - 31
                mantHi = (uint)((bits >> 32) & 0xFFFFF);    // bits 32 - 51
                exp = (uint)((bits >> 52) & 0x7FF);         // bits 52 - 62
                sign = (uint)((bits >> 63) & 0x1);          // bit 63
            }
            else {
                sign = (uint)(bits & 0x1);                  // bit 0
                exp = (uint)((bits >> 1) & 0x7FF);          // bits 1 - 11
                mantHi = (uint)((bits >> 12) & 0xFFFFF);    // bits 12 - 31
                mantLo = (uint)(bits >> 32);                // bits 32 - 63
            }
        }

        static uint Low32 (ulong value) {
            return (uint)value;
        }

        static uint High32 (ulong value) {
            return (uint)((value & 0xFFFFFFFF00000000) >> 32);
        }

        struct Number {
            public int Precision;
            public int Scale;
            public int Sign;
            public char* Digits;

            // useful for debugging
            public override string ToString () {
                return new string(Digits);
            }
        }

        const int MaxNumberDigits = 50;
        const int MaxFloatingDigits = 352;
        const int Int32Precision = 10;
        const int UInt32Precision = 10;
        const int Int64Precision = 19;
        const int UInt64Precision = 20;
        const int FloatPrecision = 7;
        const int DoublePrecision = 15;
        const int DecimalPrecision = 29;
        const int ScaleNaN = unchecked((int)0x80000000);
        const int ScaleInf = 0x7FFFFFFF;
        const int OneBillion = 1000000000;
    }
}
