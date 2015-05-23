// This file is auto-generated from the Arg.tt T4 template.

// The types here are used to forward arguments through to the string
// formatter routine without introducing any copying of the argument
// (if it's a value type) and preserving its statically known type via
// the generic type parameters.

// The switch statement in each Format() method looks ugly but gets
// translated by the compiler into a nice direct jump table.

namespace System.Text.Formatting {
    unsafe struct Arg1<T0> : IArgSet {
        IntPtr t0;

        public int Count => 1;

        public Arg1 (TypedReference t0) {
            this.t0 = *(IntPtr*)&t0;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
            }
        }
    }

    unsafe struct Arg2<T0, T1> : IArgSet {
        IntPtr t0;
        IntPtr t1;

        public int Count => 2;

        public Arg2 (TypedReference t0, TypedReference t1) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
            }
        }
    }

    unsafe struct Arg3<T0, T1, T2> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;

        public int Count => 3;

        public Arg3 (TypedReference t0, TypedReference t1, TypedReference t2) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
            }
        }
    }

    unsafe struct Arg4<T0, T1, T2, T3> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;
        IntPtr t3;

        public int Count => 4;

        public Arg4 (TypedReference t0, TypedReference t1, TypedReference t2, TypedReference t3) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
            this.t3 = *(IntPtr*)&t3;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
                case 3: formatter.AppendGeneric<T3>(t3); break;
            }
        }
    }

    unsafe struct Arg5<T0, T1, T2, T3, T4> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;
        IntPtr t3;
        IntPtr t4;

        public int Count => 5;

        public Arg5 (TypedReference t0, TypedReference t1, TypedReference t2, TypedReference t3, TypedReference t4) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
            this.t3 = *(IntPtr*)&t3;
            this.t4 = *(IntPtr*)&t4;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
                case 3: formatter.AppendGeneric<T3>(t3); break;
                case 4: formatter.AppendGeneric<T4>(t4); break;
            }
        }
    }

    unsafe struct Arg6<T0, T1, T2, T3, T4, T5> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;
        IntPtr t3;
        IntPtr t4;
        IntPtr t5;

        public int Count => 6;

        public Arg6 (TypedReference t0, TypedReference t1, TypedReference t2, TypedReference t3, TypedReference t4, TypedReference t5) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
            this.t3 = *(IntPtr*)&t3;
            this.t4 = *(IntPtr*)&t4;
            this.t5 = *(IntPtr*)&t5;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
                case 3: formatter.AppendGeneric<T3>(t3); break;
                case 4: formatter.AppendGeneric<T4>(t4); break;
                case 5: formatter.AppendGeneric<T5>(t5); break;
            }
        }
    }

    unsafe struct Arg7<T0, T1, T2, T3, T4, T5, T6> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;
        IntPtr t3;
        IntPtr t4;
        IntPtr t5;
        IntPtr t6;

        public int Count => 7;

        public Arg7 (TypedReference t0, TypedReference t1, TypedReference t2, TypedReference t3, TypedReference t4, TypedReference t5, TypedReference t6) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
            this.t3 = *(IntPtr*)&t3;
            this.t4 = *(IntPtr*)&t4;
            this.t5 = *(IntPtr*)&t5;
            this.t6 = *(IntPtr*)&t6;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
                case 3: formatter.AppendGeneric<T3>(t3); break;
                case 4: formatter.AppendGeneric<T4>(t4); break;
                case 5: formatter.AppendGeneric<T5>(t5); break;
                case 6: formatter.AppendGeneric<T6>(t6); break;
            }
        }
    }

    unsafe struct Arg8<T0, T1, T2, T3, T4, T5, T6, T7> : IArgSet {
        IntPtr t0;
        IntPtr t1;
        IntPtr t2;
        IntPtr t3;
        IntPtr t4;
        IntPtr t5;
        IntPtr t6;
        IntPtr t7;

        public int Count => 8;

        public Arg8 (TypedReference t0, TypedReference t1, TypedReference t2, TypedReference t3, TypedReference t4, TypedReference t5, TypedReference t6, TypedReference t7) {
            this.t0 = *(IntPtr*)&t0;
            this.t1 = *(IntPtr*)&t1;
            this.t2 = *(IntPtr*)&t2;
            this.t3 = *(IntPtr*)&t3;
            this.t4 = *(IntPtr*)&t4;
            this.t5 = *(IntPtr*)&t5;
            this.t6 = *(IntPtr*)&t6;
            this.t7 = *(IntPtr*)&t7;
        }

        public void Format (StringFormatter formatter, int index) {
            switch (index) {
                case 0: formatter.AppendGeneric<T0>(t0); break;
                case 1: formatter.AppendGeneric<T1>(t1); break;
                case 2: formatter.AppendGeneric<T2>(t2); break;
                case 3: formatter.AppendGeneric<T3>(t3); break;
                case 4: formatter.AppendGeneric<T4>(t4); break;
                case 5: formatter.AppendGeneric<T5>(t5); break;
                case 6: formatter.AppendGeneric<T6>(t6); break;
                case 7: formatter.AppendGeneric<T7>(t7); break;
            }
        }
    }

}