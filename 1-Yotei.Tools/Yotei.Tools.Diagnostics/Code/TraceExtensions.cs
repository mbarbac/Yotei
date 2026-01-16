namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class TraceExtensions
{
    static object SyncRoot { get; } = new();

    extension(TraceListenerCollection source)
    {
        /// <summary>
        /// Gets the collection of registered trace listeners that are console-alike ones.
        /// </summary>
        public IEnumerable<TraceListener> ConsoleListeners
        {
            get
            {
                Monitor.Enter(SyncRoot);
                try
                {
                    foreach (var item in source)
                        if (item is TextWriterTraceListener temp &&
                            ReferenceEquals(Console.Out, temp.Writer))
                            yield return temp;
                }
                finally { Monitor.Exit(SyncRoot); }
            }
        }

        /// <summary>
        /// Returns the first registered console-alike listener, ensuring that at least one exist.
        /// </summary>
        /// <returns></returns>
        public TraceListener EnsureConsoleListener()
        {
            lock (SyncRoot)
            {
                var item = source.ConsoleListeners.FirstOrDefault();
                if (item is null)
                {
                    item = new TextWriterTraceListener(Console.Out);
                    source.Add(item);
                }
                return item;
            }
        }

        /// <summary>
        /// Removes from this collection the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public void RemoveRange(TraceListenerCollection range)
        {
            ArgumentNullException.ThrowIfNull(range);
            lock (SyncRoot)
            { foreach (TraceListener item in range) source.Remove(item); }
        }

        /// <summary>
        /// Removes from this collection the elements of the given range.
        /// </summary>
        /// <param name="range"></param>
        public void RemoveRange(IEnumerable<TraceListener> range)
        {
            ArgumentNullException.ThrowIfNull(range);
            lock (SyncRoot)
            { foreach (var item in range) source.Remove(item); }
        }
    }
}