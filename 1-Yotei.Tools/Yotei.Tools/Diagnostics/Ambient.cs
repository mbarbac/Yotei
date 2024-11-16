namespace Yotei.Tools.Diagnostics;

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
}