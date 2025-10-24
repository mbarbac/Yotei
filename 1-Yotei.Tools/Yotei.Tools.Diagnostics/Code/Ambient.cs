#pragma warning disable CA1510

namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Provides access to ambient capabilities.
/// </summary>
public static class Ambient
{
    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// ones. This method uses by default a cached value that can be recomputed if needed.
    /// </summary>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsConsoleListener(
        bool recompute = false) => IsConsoleListener(out _, recompute);

    /// <summary>
    /// Determines if there is a console-alike listener registered in the collection of trace
    /// ones and, if so, returns the first one found in the our argument. This method uses by
    /// default a cached value that can be recomputed if needed.
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
    static TraceListener? Listener = null;
    static bool Computed = false;
    readonly static object Lock = new();

    // ----------------------------------------------------

    /// <summary>
    /// Enumerates the console-alike listeners registered in the collection of trace ones.
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

    // ----------------------------------------------------

    /// <summary>
    /// Gets the first console-alike listener registered in the collection of trace ones, creating
    /// one if there was none.
    /// </summary>
    /// <returns></returns>
    public static TraceListener GetOrAddConsoleListener()
    {
        lock (Lock)
        {
            var item = GetConsoleListeners().FirstOrDefault();
            if (item is null)
            {
                item = new TextWriterTraceListener(Console.Out);
                Trace.Listeners.Add(item);
            }
            return item;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Registers the trace listeners in the given collection into the trace listerners one.
    /// </summary>
    /// <param name="range"></param>
    public static void AddListeners(IEnumerable<TraceListener> range)
    {
        if (range is null) throw new ArgumentNullException(nameof(range));
        lock (Lock)
        {
            foreach (var item in range) Trace.Listeners.Add(item);
        }
    }

    /// <summary>
    /// Removes the trace listeners in the given collection from the trace listerners one.
    /// </summary>
    /// <param name="range"></param>
    public static void RemoveListeners(IEnumerable<TraceListener> range)
    {
        if (range is null) throw new ArgumentNullException(nameof(range));
        lock (Lock)
        {
            foreach (var item in range) Trace.Listeners.Remove(item);
        }
    }
}