namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class TraceExtensions
{
    extension(TraceListener source)
    {
        /// <summary>
        /// Determines if this listerner is a console-alike one, or not.
        /// </summary>
        public bool IsConsoleListener =>
            source is TextWriterTraceListener temp && ReferenceEquals(Console.Out, temp.Writer);
    }

    // ----------------------------------------------------

    static object SyncRoot { get; } = new();

    extension(TraceListenerCollection source)
    {
        /// <summary>
        /// Determines if this collection has any console listeners registered into it.
        /// </summary>
        public bool HasConsoleListeners
        {
            get
            {
                foreach (TraceListener temp in source) if (temp.IsConsoleListener) return true;
                return false;
            }
        }

        /// <summary>
        /// Returns the first registered console-alike listener, or adds and returns a new one.
        /// </summary>
        /// <returns></returns>
        public TraceListener EnsureConsoleListener()
        {
            lock (SyncRoot)
            {
                foreach (TraceListener temp in source) if (temp.IsConsoleListener) return temp;

                var item = new TextWriterTraceListener(Console.Out);
                source.Add(item);
                return item;
            }
        }
    }
}