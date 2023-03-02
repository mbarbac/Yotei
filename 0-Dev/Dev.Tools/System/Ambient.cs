namespace Dev.Tools;

// ========================================================
/// <summary>
/// Provides access to ambient elements.
/// </summary>
public static class Ambient
{
    /// <summary>
    /// Determines whether we are in DEBUG mode, or not.
    /// </summary>
    public static bool IsDebug =>
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>
    /// Determines if there is a <see cref="ConsoleTraceListener"/> listener among the registered
    /// trace listeners, or not.
    /// </summary>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsConsoleListener(bool recompute = false)
    {
        if (!_IsConsoleListenerInitialized || recompute)
        {
            _IsConsoleListenerInitialized = true;
            _IsConsoleListener = false;

            foreach (var item in Trace.Listeners)
            {
                if (item.GetType().IsAssignableTo(typeof(ConsoleTraceListener)))
                {
                    _IsConsoleListener = true;
                    break;
                }
            }
        }
        return _IsConsoleListener;
    }
    static bool _IsConsoleListener = default!;
    static bool _IsConsoleListenerInitialized = false;
}