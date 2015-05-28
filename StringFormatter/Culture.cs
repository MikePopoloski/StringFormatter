using System.Globalization;

namespace System.Text.Formatting {
    // caches formatting information from culture data
    // some of the accessors on NumberFormatInfo allocate copies of their data
    sealed class CachedCulture {
        public readonly CultureInfo Culture;

        public readonly NumberFormatData CurrencyData;
        public readonly NumberFormatData FixedData;
        public readonly NumberFormatData NumberData;
        public readonly NumberFormatData ScientificData;
        public readonly NumberFormatData PercentData;

        public readonly string CurrencyNegativePattern;
        public readonly string CurrencyPositivePattern;
        public readonly string CurrencySymbol;

        public readonly string NumberNegativePattern;
        public readonly string NumberPositivePattern;

        public readonly string PercentNegativePattern;
        public readonly string PercentPositivePattern;
        public readonly string PercentSymbol;

        public readonly string NegativeSign;
        public readonly string PositiveSign;

        public readonly string NaN;
        public readonly string PositiveInfinity;
        public readonly string NegativeInfinity;

        public readonly int DecimalBufferSize;

        public CachedCulture (CultureInfo culture) {
            Culture = culture;

            var info = culture.NumberFormat;
            CurrencyData = new NumberFormatData(
                info.CurrencyDecimalDigits,
                info.NegativeSign,
                info.CurrencyDecimalSeparator,
                info.CurrencyGroupSeparator,
                info.CurrencyGroupSizes,
                info.CurrencySymbol.Length
            );

            FixedData = new NumberFormatData(
                info.NumberDecimalDigits,
                info.NegativeSign,
                info.NumberDecimalSeparator,
                null,
                null,
                0
            );

            NumberData = new NumberFormatData(
                info.NumberDecimalDigits,
                info.NegativeSign,
                info.NumberDecimalSeparator,
                info.NumberGroupSeparator,
                info.NumberGroupSizes,
                0
            );

            ScientificData = new NumberFormatData(
                6,
                info.NegativeSign,
                info.NumberDecimalSeparator,
                null,
                null,
                info.NegativeSign.Length + info.PositiveSign.Length * 2 // for number and exponent
            );

            PercentData = new NumberFormatData(
                info.PercentDecimalDigits,
                info.NegativeSign,
                info.PercentDecimalSeparator,
                info.PercentGroupSeparator,
                info.PercentGroupSizes,
                info.PercentSymbol.Length
            );

            CurrencyNegativePattern = NegativeCurrencyFormats[info.CurrencyNegativePattern];
            CurrencyPositivePattern = PositiveCurrencyFormats[info.CurrencyPositivePattern];
            CurrencySymbol = info.CurrencySymbol;
            NumberNegativePattern = NegativeNumberFormats[info.NumberNegativePattern];
            NumberPositivePattern = PositiveNumberFormat;
            PercentNegativePattern = NegativePercentFormats[info.PercentNegativePattern];
            PercentPositivePattern = PositivePercentFormats[info.PercentPositivePattern];
            PercentSymbol = info.PercentSymbol;
            NegativeSign = info.NegativeSign;
            PositiveSign = info.PositiveSign;
            NaN = info.NaNSymbol;
            PositiveInfinity = info.PositiveInfinitySymbol;
            NegativeInfinity = info.NegativeInfinitySymbol;
            DecimalBufferSize =
                NumberFormatData.MinBufferSize +
                info.NumberDecimalSeparator.Length +
                (NegativeSign.Length + PositiveSign.Length) * 2;
        }

        static readonly string[] PositiveCurrencyFormats = {
            "$#", "#$", "$ #", "# $"
        };

        static readonly string[] NegativeCurrencyFormats = {
            "($#)", "-$#", "$-#", "$#-",
            "(#$)", "-#$", "#-$", "#$-",
            "-# $", "-$ #", "# $-", "$ #-",
            "$ -#", "#- $", "($ #)", "(# $)"
        };

        static readonly string[] PositivePercentFormats = {
            "# %", "#%", "%#", "% #"
        };

        static readonly string[] NegativePercentFormats = {
            "-# %", "-#%", "-%#",
            "%-#", "%#-",
            "#-%", "#%-",
            "-% #", "# %-", "% #-",
            "% -#", "#- %"
        };

        static readonly string[] NegativeNumberFormats = {
            "(#)", "-#", "- #", "#-", "# -",
        };

        static readonly string PositiveNumberFormat = "#";
    }

    // contains format information for a specific kind of format string
    // e.g. (fixed, number, currency)
    sealed class NumberFormatData {
        readonly int bufferLength;
        readonly int perDigitLength;

        public readonly int DecimalDigits;
        public readonly string NegativeSign;
        public readonly string DecimalSeparator;
        public readonly string GroupSeparator;
        public readonly int[] GroupSizes;

        public NumberFormatData (int decimalDigits, string negativeSign, string decimalSeparator, string groupSeparator, int[] groupSizes, int extra) {
            DecimalDigits = decimalDigits;
            NegativeSign = negativeSign;
            DecimalSeparator = decimalSeparator;
            GroupSeparator = groupSeparator;
            GroupSizes = groupSizes;

            bufferLength = MinBufferSize;
            bufferLength += NegativeSign.Length;
            bufferLength += DecimalSeparator.Length;
            bufferLength += extra;

            if (GroupSeparator != null)
                perDigitLength = GroupSeparator.Length;
        }

        public int GetBufferSize (ref int maxDigits, int scale) {
            if (maxDigits < 0)
                maxDigits = DecimalDigits;

            var digitCount = scale >= 0 ? scale + maxDigits : 0;
            long len = bufferLength;

            // calculate buffer size
            len += digitCount;
            len += perDigitLength * digitCount;
            return checked((int)len);
        }

        internal const int MinBufferSize = 105;
    }
}
