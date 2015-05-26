using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    // TODO: clean this up
    public unsafe struct StringView {
        public static readonly StringView Empty = new StringView();

        public readonly char* Data;
        public readonly int Length;

        public bool IsEmpty {
            get { return Length == 0; }
        }

        public StringView (char* data, int length) {
            Data = data;
            Length = length;
        }

        public static bool operator ==(StringView lhs, string rhs) {
            var count = lhs.Length;
            if (count != rhs.Length)
                return false;

            fixed (char* r = rhs)
            {
                var lhsPtr = lhs.Data;
                var rhsPtr = r;
                for (int i = 0; i < count; i++) {
                    if (*lhsPtr++ != *rhsPtr++)
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(StringView lhs, string rhs) {
            return !(lhs == rhs);
        }
    }
}
