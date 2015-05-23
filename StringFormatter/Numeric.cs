using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    // Most of the implementation of this file was ported from the native versions built into the CLR
    // See: https://github.com/dotnet/coreclr/blob/838807429a0828a839958e3b7d392d65886c8f2e/src/classlibnative/bcltype/number.cpp
    // Also see: https://github.com/dotnet/coreclr/blob/02084af832c2900cf6eac2a168c41f261409be97/src/mscorlib/src/System/Number.cs

    unsafe static class Numeric {
        public static void FormatInt32 (StringFormatter formatter, int value, string specifier, CachedCulture culture) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);
            switch (fmt) {
                case 'G':
                    if (digits > 0)
                        goto default;
                    else
                        goto case 'D';

                case 'D':
                    Int32ToDecStr(formatter, value, digits, culture.NegativeSign);
                    break;

                case 'X':
                    break;

                default:
                    var number = new Number();
                    var buffer = stackalloc char[MaxNumberDigits + 1];
                    number.Digits = buffer;
                    Int32ToNumber(value, ref number);
                    if (fmt != 0)
                        NumberToString(formatter, ref number, fmt, digits, culture);
                    break;
            }
        }

        static void NumberToString (StringFormatter formatter, ref Number number, char format, int maxDigits, CachedCulture culture, bool isDecimal = false) {
            switch (format) {
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

                        RoundNumber(ref number, number.Scale + maxDigits);

                        var buffer = stackalloc char[bufferSize];
                        var ptr = buffer;
                        if (number.Sign > 0)
                            AppendString(&ptr, cultureData.NegativeSign);

                        ptr = FormatScientific(
                            buffer,
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
                            throw new ArgumentOutOfRangeException();
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

        static void Int32ToDecStr (StringFormatter formatter, int value, int digits, string negativeSign) {
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

            var buffer = stackalloc char[bufferLength * sizeof(char)];
            var p = Int32ToDecChars(buffer + bufferLength, value >= 0 ? (uint)value : (uint)-value, digits);
            if (value < 0) {
                // add the negative sign
                for (int i = negativeLength - 1; i >= 0; i--)
                    *(--p) = negativeSign[i];
            }

            formatter.Append(p, (int)(buffer + bufferLength - p));
        }

        static char* Int32ToDecChars (char* p, uint value, int digits) {
            while (--digits >= 0 || value != 0) {
                *--p = (char)(value % 10 + '0');
                value /= 10;
            }
            return p;
        }

        // looks for standard numeric format strings: single letter followed by the number of digits
        // see: https://msdn.microsoft.com/en-us/library/dwhawy9k%28v=vs.110%29.aspx
        static char ParseFormatSpecifier (string specifier, out int digits) {
            if (string.IsNullOrEmpty(specifier)) {
                digits = -1;
                return 'G';
            }

            fixed (char* ptr = specifier)
            {
                char* curr = ptr;
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
                        // ANDing with 0xFFDF has the effect of uppercasing the character
                        digits = n;
                        return (char)(first & 0xFFDF);
                    }
                }
            }

            digits = -1;
            return (char)0;
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

        struct Number {
            public int Precision;
            public int Scale;
            public int Sign;
            public char* Digits;
        }

        const int MaxNumberDigits = 50;
        const int Int32Precision = 10;
    }
}
