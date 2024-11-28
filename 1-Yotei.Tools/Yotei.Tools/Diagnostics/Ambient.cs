﻿namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class Ambient
{
    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// ones, or not.
    /// </summary>
    /// <returns></returns>
    public static bool IsConsoleListener() => IsConsoleListener(out _);

    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// ones, or not. Returns in the out argument the first listener found, if any, or null.
    /// </summary>
    /// <param name="listener"></param>
    /// <returns></returns>
    public static bool IsConsoleListener([NotNullWhen(true)] out TraceListener? listener)
    {
        if (_Computed)
        {
            listener = _Listener;
            return _Listener is not null;
        }

        _Computed = true;
        _Listener = null;

        foreach (var item in Trace.Listeners)
        {
            if (item is TextWriterTraceListener temp && ReferenceEquals(Console.Out, temp.Writer))
            {
                _Listener = listener = temp;
                return true;
            }
        }

        listener = null;
        return false;
    }

    /// <summary>
    /// Enforces to recompute the console listeners.
    /// </summary>
    public static void RecomputeConsoleListener() => _Computed = false;

    static TraceListener? _Listener = null;
    static bool _Computed = false;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    internal static string Header(int size)
    {
        switch (size)
        {
            case 0: return Header0;
            case 1: return Header1;
            case 2: return Header2;
            case 3: return Header3;
            case 4: return Header4;
            case 5: return Header5;
            case 6: return Header6;
            case 7: return Header7;
            case 8: return Header8;
            case 9: return Header9;
        }

        if (!Headers.TryGetValue(size, out var header)) Headers.Add(size, header = new(' ', size));
        return header;
    }

    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(' ', 1);
    readonly static string Header2 = new(' ', 2);
    readonly static string Header3 = new(' ', 3);
    readonly static string Header4 = new(' ', 4);
    readonly static string Header5 = new(' ', 5);
    readonly static string Header6 = new(' ', 6);
    readonly static string Header7 = new(' ', 7);
    readonly static string Header8 = new(' ', 8);
    readonly static string Header9 = new(' ', 9);

    readonly static Dictionary<int, string> Headers = [];
}