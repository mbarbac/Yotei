namespace Dev.Tools;

// ========================================================
/// <summary>
/// Provides access to ambient elements.
/// </summary>
public static class Ambient
{
    /// <summary>
    /// Determines whether a debugger is attached to this process.
    /// </summary>
    public static bool IsDebugAttached => Debugger.IsAttached;

    /// <summary>
    /// Determines if debug output is emitted to the console, or not.
    /// </summary>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public static bool IsDebugOnConsole(bool recompute = false)
    {
        if (!_DebugOnConsoleInitialized || recompute)
        {
            _DebugOnConsoleInitialized = true;
            _IsDebugOnConsole = false;

            foreach (var item in Trace.Listeners)
                if (item.GetType().IsAssignableTo(typeof(ConsoleTraceListener)))
                    _IsDebugOnConsole = true;
        }
        return _IsDebugOnConsole;
    }

    static bool _IsDebugOnConsole = default!;
    static bool _DebugOnConsoleInitialized = false;
}