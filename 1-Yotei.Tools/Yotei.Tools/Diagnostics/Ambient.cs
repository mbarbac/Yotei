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

    // ----------------------------------------------------

    /// <summary>
    /// Determines if there is at least one element in the given collection that matches the given
    /// predicate, or not.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Any(this TraceListenerCollection items, Predicate<TraceListener> predicate)
    {
        items.ThrowWhenNull();
        predicate.ThrowWhenNull();

        foreach (var item in items) if (predicate((TraceListener)item)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Unregisters and returns the collection of console listeners.
    /// </summary>
    /// <returns></returns>
    public static ImmutableArray<TraceListener> UnregisterConsoleListeners()
    {
        List<TraceListener> items = [];
        
        INIT:
        foreach (var item in Trace.Listeners)
        {
            if (item is TextWriterTraceListener temp && ReferenceEquals(Console.Out, temp.Writer))
            {
                items.Add(temp);
                Trace.Listeners.Remove((TraceListener)item);
                goto INIT;
            }
        }

        _Listener = null;
        _Computed = false;

        return items.ToImmutableArray();
    }

    /// <summary>
    /// Re-registers the given collection of console listerners.
    /// </summary>
    /// <param name="items"></param>
    public static void RegisterConsoleListeners(IEnumerable<TraceListener> items)
    {
        items.ThrowWhenNull();

        foreach (var item in items)
        {
            if (item is TextWriterTraceListener temp &&
                ReferenceEquals(Console.Out, temp.Writer) &&
                !Trace.Listeners.Contains(item))
            {
                Trace.Listeners.Add(item);
            }
        }
    }
}