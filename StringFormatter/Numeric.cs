using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    unsafe static class Numeric {
        public static void FormatInt32 (StringFormatter formatter, int value, string specifier, NumberFormatInfo numberFormat) {
            int digits;
            var fmt = ParseFormatSpecifier(specifier, out digits);
            switch (fmt) {
                case 'G':
                    if (digits > 0) {
                        break;
                    }
                    goto case 'D';

                case 'D':
                    Int32ToDecStr(formatter, value, digits, numberFormat.NegativeSign);
                    break;

                case 'X':
                    break;
                default:
                    break;
            }
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
    }
}
