namespace Yotei.Tools.Diagnostics;

// =============================================================
/// <summary>
/// Provides access to ambient capabilities.
/// </summary>
public static class Ambient
{
    static TraceListener? Listener = null;
    static bool Computed = false;

    /// <summary>
    /// The object used for locking access to static members of this class.
    /// </summary>
    internal static object Lock { get; } = new();

    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// listeners. If so, returns true. This method uses by default a cached value, unless recompute
    /// is requested.
    /// </summary>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsConsoleListener(
        bool recompute = false) => IsConsoleListener(out _, recompute);

    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// listeners. If so, returns true and the listener instance. This method uses by default a
    /// cached value, unless recompute is requested.
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsConsoleListener(
        [NotNullWhen(true)] out TraceListener? listener,
        bool recompute = false)
    {
        lock (Lock)
        {
            if (!Computed || recompute)
            {
                Listener = GetConsoleListeners().FirstOrDefault();
                Computed = true;
            }
            listener = Listener;
            return listener is not null;
        }
    }

    /// <summary>
    /// Returns the console-alike listeners registered in the collection of trace listeners.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<TraceListener> GetConsoleListeners()
    {
        Monitor.Enter(Lock);
        try
        {
            foreach (var item in Trace.Listeners)
            {
                if (item is TextWriterTraceListener temp &&
                    ReferenceEquals(Console.Out, temp.Writer))
                    yield return temp;
            }
        }
        finally { Monitor.Exit(Lock); }
    }

    /// <summary>
    /// Adds a console-alike listener to the collection of trace listeners, if none is already.
    /// Returns either the newly created listener, or the existing one.
    /// </summary>
    /// <returns></returns>
    public static TraceListener? AddConsoleListener() => AddConsoleListener(out _);

    /// <summary>
    /// Adds a console-alike listener to the collection of trace listeners, if none is already.
    /// Returns either the newly created listener, or the existing one, and whether the listener
    /// was created or not.
    /// </summary>
    /// <param name="created"></param>
    /// <returns></returns>
    public static TraceListener? AddConsoleListener(out bool created)
    {
        lock (Lock)
        {
            var items = GetConsoleListeners().ToArray();
            if (items.Length == 0)
            {
                var item = new TextWriterTraceListener(Console.Out);
                Trace.Listeners.Add(item);
                created = true;
                return item;
            }
            created = false;
            return items[0];
        }
    }

    /// <summary>
    /// Adds to the collection of trace listeners the listeners from the given range.
    /// </summary>
    /// <param name="range"></param>
    public static void AddListeners(IEnumerable<TraceListener> range)
    {
        lock (Lock)
        {
            range.ThrowWhenNull();
            foreach (var item in range) Trace.Listeners.Add(item);
        }
    }

    /// <summary>
    /// Removes from the collection of trace listeners the listeners from the given range.
    /// </summary>
    /// <param name="range"></param>
    public static void RemoveListeners(IEnumerable<TraceListener> range)
    {
        lock (Lock)
        {
            range.ThrowWhenNull();
            foreach (var item in range) Trace.Listeners.Remove(item);
        }
    }
}