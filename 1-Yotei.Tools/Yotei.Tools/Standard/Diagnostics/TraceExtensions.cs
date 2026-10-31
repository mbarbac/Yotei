namespace Yotei.Tools;

// ========================================================
public static class TraceExtensions
{
    extension(TraceListener source)
    {
        /// <summary>
        /// Determines if this listener writes to the console or not.
        /// </summary>
        public bool IsConsoleListener =>
            source is TextWriterTraceListener temp && ReferenceEquals(Console.Out, temp.Writer);
    }

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
        /// Returns the first console-alike registered listener, creating a new one if needed.
        /// </summary>
        /// <returns></returns>
        public TraceListener EnsureConsoleListener()
        {
            lock (source)
            {
                foreach (TraceListener temp in source) if (temp.IsConsoleListener) return temp;

                var item = new TextWriterTraceListener(Console.Out);
                source.Add(item);
                return item;
            }
        }
    }
}