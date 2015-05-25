using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    public unsafe struct StringView {
        public static readonly StringView Empty = new StringView();

        public readonly char* Data;
        public readonly int Length;

        public bool IsEmpty {
            get { return Length > 0; }
        }

        public StringView (char* data, int length) {
            Data = data;
            Length = length;
        }
    }
}
