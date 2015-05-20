
struct Arg1<T0> : IArgSet {
    T0 t0;

    public int Count => 1;

    public Arg1 (T0 t0) {
        this.t0 = t0;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg1<T0> arg) {
        formatter.Append(arg.t0);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg1<T0> arg);
    static readonly Accessor[] Accessors = {
        Format0
    };
}

struct Arg2<T0, T1> : IArgSet {
    T0 t0;
    T1 t1;

    public int Count => 2;

    public Arg2 (T0 t0, T1 t1) {
        this.t0 = t0;
        this.t1 = t1;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg2<T0, T1> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg2<T0, T1> arg) {
        formatter.Append(arg.t1);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg2<T0, T1> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1
    };
}

struct Arg3<T0, T1, T2> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;

    public int Count => 3;

    public Arg3 (T0 t0, T1 t1, T2 t2) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg3<T0, T1, T2> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg3<T0, T1, T2> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg3<T0, T1, T2> arg) {
        formatter.Append(arg.t2);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg3<T0, T1, T2> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2
    };
}

struct Arg4<T0, T1, T2, T3> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;
    T3 t3;

    public int Count => 4;

    public Arg4 (T0 t0, T1 t1, T2 t2, T3 t3) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg4<T0, T1, T2, T3> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg4<T0, T1, T2, T3> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg4<T0, T1, T2, T3> arg) {
        formatter.Append(arg.t2);
    }

    static void Format3 (StringFormatter formatter, ref Arg4<T0, T1, T2, T3> arg) {
        formatter.Append(arg.t3);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg4<T0, T1, T2, T3> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2,
        Format3
    };
}

struct Arg5<T0, T1, T2, T3, T4> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;
    T3 t3;
    T4 t4;

    public int Count => 5;

    public Arg5 (T0 t0, T1 t1, T2 t2, T3 t3, T4 t4) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
        this.t4 = t4;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg) {
        formatter.Append(arg.t2);
    }

    static void Format3 (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg) {
        formatter.Append(arg.t3);
    }

    static void Format4 (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg) {
        formatter.Append(arg.t4);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg5<T0, T1, T2, T3, T4> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2,
        Format3,
        Format4
    };
}

struct Arg6<T0, T1, T2, T3, T4, T5> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;
    T3 t3;
    T4 t4;
    T5 t5;

    public int Count => 6;

    public Arg6 (T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
        this.t4 = t4;
        this.t5 = t5;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t2);
    }

    static void Format3 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t3);
    }

    static void Format4 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t4);
    }

    static void Format5 (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg) {
        formatter.Append(arg.t5);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg6<T0, T1, T2, T3, T4, T5> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2,
        Format3,
        Format4,
        Format5
    };
}

struct Arg7<T0, T1, T2, T3, T4, T5, T6> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;
    T3 t3;
    T4 t4;
    T5 t5;
    T6 t6;

    public int Count => 7;

    public Arg7 (T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
        this.t4 = t4;
        this.t5 = t5;
        this.t6 = t6;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t2);
    }

    static void Format3 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t3);
    }

    static void Format4 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t4);
    }

    static void Format5 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t5);
    }

    static void Format6 (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg) {
        formatter.Append(arg.t6);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg7<T0, T1, T2, T3, T4, T5, T6> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2,
        Format3,
        Format4,
        Format5,
        Format6
    };
}

struct Arg8<T0, T1, T2, T3, T4, T5, T6, T7> : IArgSet {
    T0 t0;
    T1 t1;
    T2 t2;
    T3 t3;
    T4 t4;
    T5 t5;
    T6 t6;
    T7 t7;

    public int Count => 8;

    public Arg8 (T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
        this.t0 = t0;
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
        this.t4 = t4;
        this.t5 = t5;
        this.t6 = t6;
        this.t7 = t7;
    }

    public void Format (StringFormatter formatter, int index) {
        Accessors[index](formatter, ref this);
    }

    static void Format0 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t0);
    }

    static void Format1 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t1);
    }

    static void Format2 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t2);
    }

    static void Format3 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t3);
    }

    static void Format4 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t4);
    }

    static void Format5 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t5);
    }

    static void Format6 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t6);
    }

    static void Format7 (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg) {
        formatter.Append(arg.t7);
    }

    delegate void Accessor (StringFormatter formatter, ref Arg8<T0, T1, T2, T3, T4, T5, T6, T7> arg);
    static readonly Accessor[] Accessors = {
        Format0,
        Format1,
        Format2,
        Format3,
        Format4,
        Format5,
        Format6,
        Format7
    };
}

