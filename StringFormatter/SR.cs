using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    // currently just contains some hardcoded exception messages
    static class SR {
        public const string InvalidGroupSizes = "Invalid group sizes in NumberFormatInfo.";
        public const string UnknownFormatSpecifier = "Unknown format specifier '{0}'.";
        public const string ArgIndexOutOfRange = "No format argument exists for index '{0}'.";
        public const string TypeNotFormattable = "Type '{0}' is not a built-in type, does not implement IStringFormattable, and no custom formatter was found for it.";
        public const string InvalidFormatString = "Invalid format string.";
    }
}
