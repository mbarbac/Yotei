namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Provides access to ambient elements.
/// </summary>
public static class Ambient
{
    /// <summary>
    /// Determines if there is a console-alike listener already registeres in the collection
    /// of trace listeners, or not. If the <paramref name="recompute"/> argument is set to true,
    /// then the cached value is discarded and the value of this property is recomputed.
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