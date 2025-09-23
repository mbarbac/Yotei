namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Provides access to ambient capabilities.
/// </summary>
public static class Ambient
{
    static TraceListener? _Listener = null;
    static bool _Computed = false;

    /// <summary>
    /// Determines if there is a console-alike listener registered in the trace listeners.
    /// </summary>
    /// <returns></returns>
    public static bool IsConsoleListener() => IsConsoleListener(out _);

    /// <summary>
    /// Determines if there is a console-alike listener registered in the trace listeners. If
    /// recompute is requested, then the collection of trace listeners is seeked for a matching
    /// one. Otherwise, a cached value is used, if any.
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsConsoleListener(
        [NotNullWhen(true)] out TraceListener? listener,
        bool recompute = false)
    {
        if (!_Computed || recompute)
        {
            var items = GetConsoleListeners().ToArray();
            _Listener = items.Length > 0 ? items[0] : null;
            _Computed = true;
        }

        listener = _Listener;
        return _Listener is not null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the console-alike listeners registerd in the colleciton of trace ones.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<TraceListener> GetConsoleListeners()
    {
        foreach (var item in Trace.Listeners)
        {
            if (item is TextWriterTraceListener temp &&
                ReferenceEquals(Console.Out, temp.Writer))
                yield return temp;
        }
    }

    /// <summary>
    /// Adds to the collection of trace listeners a console-alike one, provided any is registered
    /// yet. Returns the added listener, or null otherwise.
    /// </summary>
    /// <returns></returns>
    public static TraceListener? AddNewConsoleListener()
    {
        var items = GetConsoleListeners().ToArray();
        if (items.Length == 0)
        {
            var item = new TextWriterTraceListener(Console.Out);
            Trace.Listeners.Add(item);
            return item;
        }
        return null;
    }

    /// <summary>
    /// Removes from the collection of trace listeners all the console-alike ones.
    /// </summary>
    /// <param name="range"></param>
    public static void RemoveRangeListeners(IEnumerable<TraceListener> range)
    {
        range.ThrowWhenNull();
        foreach (var item in range) Trace.Listeners.Remove(item);
    }
}