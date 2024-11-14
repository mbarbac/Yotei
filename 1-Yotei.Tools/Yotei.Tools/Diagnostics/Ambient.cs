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
        foreach (var item in Trace.Listeners)
        {
            if (item is TextWriterTraceListener temp && ReferenceEquals(Console.Out, temp.Writer))
            {
                listener = temp;
                return true;
            }
        }

        listener = null;
        return false;
    }
}