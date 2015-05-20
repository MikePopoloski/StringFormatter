using System;
using System.Collections.Generic;

class Program {
    static void Main () {
        var formatter = new StringFormatter();
        formatter.Append("Foo {0,3} and bar!! {1,-5}bah", 3, -8);

        Console.WriteLine(formatter.ToString());
    }
}

class StringFormatter {
    List<char> mainBuffer = new List<char>();
    List<char> padBuffer = new List<char>();
    List<char> currentBuffer;
    int padding;

    public StringFormatter () {
        currentBuffer = mainBuffer;
    }

    public void Append (char c) {
        currentBuffer.Add(c);
        padding -= 1;
    }

    public void Append (char c, int count) {
        for (int i = 0; i < count; i++)
            currentBuffer.Add(c);
        padding -= count;
    }

    public void Append (int i) {
        Append(i.ToString());
    }

    public void Append (string str) {
        foreach (var c in str)
            currentBuffer.Add(c);
        padding -= str.Length;
    }

    public void Append<T>(T value) {
        // this looks gross, but T is known at JIT-time so this call tree
        // is hopefully going to get optimized down to a direct call
        if (typeof(T) == typeof(int))
            Append(value as int? ?? 0);
        else if (typeof(T) == typeof(string))
            Append(value as string);
        else if (typeof(T) == typeof(char))
            Append(value as char? ?? 0);
        else
            throw new InvalidOperationException();
    }

    public override string ToString () {
        return string.Concat(mainBuffer);
    }

    public void Append<T0>(string format, T0 arg0) {
        var args = new Arg1<T0>(arg0);
        AppendSet(format, ref args);
    }

    public void Append<T0, T1>(string format, T0 arg0, T1 arg1) {
        var args = new Arg2<T0, T1>(arg0, arg1);
        AppendSet(format, ref args);
    }

    public void AppendSet<T>(string format, ref T args) where T : IArgSet {
        unsafe
        {
            fixed (char* formatPtr = format)
            {
                var curr = formatPtr;
                var end = curr + format.Length;
                while (AppendSegment(ref curr, end, ref args)) ;
            }
        }
    }

    unsafe bool AppendSegment<T>(ref char* currRef, char* end, ref T args) where T : IArgSet {
        char* curr = currRef;
        char c = '\x0';
        while (curr < end) {
            c = *curr++;
            if (c == '}') {
                // check for escape character for }}
                if (curr < end && *curr == '}')
                    curr++;
                else
                    ThrowError();
            }

            if (c == '{') {
                // check for escape character for {{
                if (curr == end)
                    ThrowError();
                else if (*curr == '{')
                    curr++;
                else
                    break;
            }

            Append(c);
        }

        if (curr == end)
            return false;

        var index = ParseNum(ref curr, end, MaxArgs);
        if (index >= args.Count)
            ThrowError();

        // check for a spacing specifier
        c = SkipWhitespace(ref curr, end);
        int width = 0;
        var leftJustify = false;
        if (c == ',') {
            curr++;
            c = SkipWhitespace(ref curr, end);

            // spacing can be left-justified
            if (c == '-') {
                leftJustify = true;
                curr++;
                if (curr == end)
                    ThrowError();
            }

            width = ParseNum(ref curr, end, MaxSpacing);
            c = SkipWhitespace(ref curr, end);
        }

        // check for format specifier

        if (c != '}')
            ThrowError();
        curr++;
        currRef = curr;

        try {
            padding = width;
            if (width > 0 && !leftJustify) {
                // right justification requires a temporary
                // buffer so that we can add leading spaces
                padBuffer.Clear();
                currentBuffer = padBuffer;
            }
            args.Format(this, index);
        }
        catch {
            // reset to a good state so that if somebody up the
            // callstack eats this exception, the formatter is
            // in a usable state
            currentBuffer = mainBuffer;
            throw;
        }

        // finish off padding, if necessary
        if (padding > 0) {
            currentBuffer = mainBuffer;
            Append(' ', padding);
            if (!leftJustify)
                mainBuffer.AddRange(padBuffer);
        }

        return true;
    }

    unsafe static int ParseNum (ref char* currRef, char* end, int maxValue) {
        char* curr = currRef;
        char c = *curr;
        if (c < '0' || c > '9')
            ThrowError();

        int value = 0;
        do {
            value = value * 10 + c - '0';
            curr++;
            if (curr == end)
                ThrowError();
            c = *curr;
        } while (c >= '0' && c <= '9' && value < maxValue);

        currRef = curr;
        return value;
    }

    unsafe static char SkipWhitespace (ref char* currRef, char* end) {
        char* curr = currRef;
        while (curr < end && *curr == ' ') curr++;

        if (curr == end)
            ThrowError();

        currRef = curr;
        return *curr;
    }

    static void ThrowError () {
        throw new FormatException();
    }

    const int MaxArgs = 64;
    const int MaxSpacing = 1000000;
}

interface IArgSet {
    int Count { get; }
    void Format (StringFormatter formatter, int index);
}

// need a better name for this
interface IStringifiable {
    void Format (StringFormatter formatter);
}