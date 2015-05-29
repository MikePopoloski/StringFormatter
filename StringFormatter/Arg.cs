// This file is auto-generated from the Arg.tt T4 template.

// The types here are used to forward arguments through to the string
// formatter routine without introducing any copying of the argument
// (if it's a value type) and preserving its statically known type via
// the generic type parameters.

// The switch statement in each Format() method looks ugly but gets
// translated by the compiler into a nice direct jump table.

using System.Runtime.CompilerServices;

namespace System.Text.Formatting {
    /// <summary>
    /// A low-allocation version of the built-in <see cref="StringBuilder"/> type.
    /// </summary>
    partial class StringBuffer {
        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>   
        public void AppendFormat<T0>(string format, T0 arg0) {
            var args = new Arg1<T0>(__makeref(arg0));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>   
        public static string Format<T0>(string format, T0 arg0) {
            var buffer = Acquire(format.Length + 8);
            buffer.AppendFormat(format, arg0);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>   
        public void AppendFormat<T0, T1>(string format, T0 arg0, T1 arg1) {
            var args = new Arg2<T0, T1>(__makeref(arg0), __makeref(arg1));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>   
        public static string Format<T0, T1>(string format, T0 arg0, T1 arg1) {
            var buffer = Acquire(format.Length + 16);
            buffer.AppendFormat(format, arg0, arg1);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>   
        public void AppendFormat<T0, T1, T2>(string format, T0 arg0, T1 arg1, T2 arg2) {
            var args = new Arg3<T0, T1, T2>(__makeref(arg0), __makeref(arg1), __makeref(arg2));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>   
        public static string Format<T0, T1, T2>(string format, T0 arg0, T1 arg1, T2 arg2) {
            var buffer = Acquire(format.Length + 24);
            buffer.AppendFormat(format, arg0, arg1, arg2);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>   
        public void AppendFormat<T0, T1, T2, T3>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
            var args = new Arg4<T0, T1, T2, T3>(__makeref(arg0), __makeref(arg1), __makeref(arg2), __makeref(arg3));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>   
        public static string Format<T0, T1, T2, T3>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
            var buffer = Acquire(format.Length + 32);
            buffer.AppendFormat(format, arg0, arg1, arg2, arg3);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>   
        public void AppendFormat<T0, T1, T2, T3, T4>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            var args = new Arg5<T0, T1, T2, T3, T4>(__makeref(arg0), __makeref(arg1), __makeref(arg2), __makeref(arg3), __makeref(arg4));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>   
        public static string Format<T0, T1, T2, T3, T4>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            var buffer = Acquire(format.Length + 40);
            buffer.AppendFormat(format, arg0, arg1, arg2, arg3, arg4);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>   
        public void AppendFormat<T0, T1, T2, T3, T4, T5>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
            var args = new Arg6<T0, T1, T2, T3, T4, T5>(__makeref(arg0), __makeref(arg1), __makeref(arg2), __makeref(arg3), __makeref(arg4), __makeref(arg5));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>   
        public static string Format<T0, T1, T2, T3, T4, T5>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
            var buffer = Acquire(format.Length + 48);
            buffer.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>
        /// <param name="arg6">A value to format.</param>   
        public void AppendFormat<T0, T1, T2, T3, T4, T5, T6>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
            var args = new Arg7<T0, T1, T2, T3, T4, T5, T6>(__makeref(arg0), __makeref(arg1), __makeref(arg2), __makeref(arg3), __makeref(arg4), __makeref(arg5), __makeref(arg6));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>
        /// <param name="arg6">A value to format.</param>   
        public static string Format<T0, T1, T2, T3, T4, T5, T6>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
            var buffer = Acquire(format.Length + 56);
            buffer.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>
        /// <param name="arg6">A value to format.</param>
        /// <param name="arg7">A value to format.</param>   
        public void AppendFormat<T0, T1, T2, T3, T4, T5, T6, T7>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) {
            var args = new Arg8<T0, T1, T2, T3, T4, T5, T6, T7>(__makeref(arg0), __makeref(arg1), __makeref(arg2), __makeref(arg3), __makeref(arg4), __makeref(arg5), __makeref(arg6), __makeref(arg7));
            AppendArgSet(format, ref args);
        }

        /// <summary>
        /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">A value to format.</param>
        /// <param name="arg1">A value to format.</param>
        /// <param name="arg2">A value to format.</param>
        /// <param name="arg3">A value to format.</param>
        /// <param name="arg4">A value to format.</param>
        /// <param name="arg5">A value to format.</param>
        /// <param name="arg6">A value to format.</param>
        /// <param name="arg7">A value to format.</param>   
        public static string Format<T0, T1, T2, T3, T4, T5, T6, T7>(string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) {
            var buffer = Acquire(format.Length + 64);
            buffer.AppendFormat(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            var result = buffer.ToString();
            Release(buffer);
            return result;
        }
    }

    unsafe struct Arg1<T0> : IArgSet {
        IntPtr t0;

        public int Count => 1;

        public Arg1 (TypedReference t0) {
            this.t0 = *(IntPtr*)&t0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
                case 3: buffer.AppendGeneric<T3>(t3, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
                case 3: buffer.AppendGeneric<T3>(t3, format); break;
                case 4: buffer.AppendGeneric<T4>(t4, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
                case 3: buffer.AppendGeneric<T3>(t3, format); break;
                case 4: buffer.AppendGeneric<T4>(t4, format); break;
                case 5: buffer.AppendGeneric<T5>(t5, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
                case 3: buffer.AppendGeneric<T3>(t3, format); break;
                case 4: buffer.AppendGeneric<T4>(t4, format); break;
                case 5: buffer.AppendGeneric<T5>(t5, format); break;
                case 6: buffer.AppendGeneric<T6>(t6, format); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Format (StringBuffer buffer, int index, StringView format) {
            switch (index) {
                case 0: buffer.AppendGeneric<T0>(t0, format); break;
                case 1: buffer.AppendGeneric<T1>(t1, format); break;
                case 2: buffer.AppendGeneric<T2>(t2, format); break;
                case 3: buffer.AppendGeneric<T3>(t3, format); break;
                case 4: buffer.AppendGeneric<T4>(t4, format); break;
                case 5: buffer.AppendGeneric<T5>(t5, format); break;
                case 6: buffer.AppendGeneric<T6>(t6, format); break;
                case 7: buffer.AppendGeneric<T7>(t7, format); break;
            }
        }
    }
}