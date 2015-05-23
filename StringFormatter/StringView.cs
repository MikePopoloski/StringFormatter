using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Formatting {
    public unsafe struct StringView {
        public string Data;

        public bool IsEmpty => string.IsNullOrEmpty(Data);
    }
}
