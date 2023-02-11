using _Debug = System.Diagnostics.Debug;
using _Console = System.Console;

namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a wrapper over the <see cref="_Console"/> class.
/// </summary>
public static class ConsoleWrapper
{
    /// <summary>
    /// Determines if the console output shall also be emitted into debug, or not.
    /// </summary>
    public static bool UseDebug { get; set; } = false;
    static bool ForDebug => Ambient.IsDebug && UseDebug;

    // ----------------------------------------------------

    /// <summary>
    /// Writes a formatted message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        if (ForDebug)
        {
            _Debug.Write(message);
            if (!Ambient.IsDebugOnConsole()) _Console.Write(message);
        }
        else _Console.Write(message);
    }

    /// <summary>
    /// Writes a formatted message, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        Write(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line.
    /// </summary>
    public static void WriteLine()
    {
        if (ForDebug)
        {
            _Debug.WriteLine(string.Empty);
            if (!Ambient.IsDebugOnConsole()) _Console.WriteLine(string.Empty);
        }
        else _Console.WriteLine(string.Empty);
    }

    /// <summary>
    /// Writes a formatted message, followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        if (ForDebug)
        {
            _Debug.WriteLine(message);
            if (!Ambient.IsDebugOnConsole()) _Console.WriteLine(message);
        }
        else _Console.WriteLine(message);
    }

    /// <summary>
    /// Writes a formatted message, using the given color, followed by a new line.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        WriteLine(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads a line from the _Console.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine()
    {
        var str = _Console.ReadLine();
        if (str != null && str.Length == 0) str = null;

        return str;
    }

    /// <summary>
    /// Reads a line from the _Console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string? ReadLine(string message, params object?[] args)
    {
        Write(message, args);
        return ReadLine();
    }

    /// <summary>
    /// Reads a line from the _Console.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string? ReadLine(Color color, string message, params object?[] args)
    {
        Write(color, message, args);
        return ReadLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next key pressed by the user. If the intercept parameter is 'true', then
    /// that key is not displayed.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept = true) => _Console.ReadKey(intercept);

    /// <summary>
    /// Returns the next key pressed by the user, or null if the wait time has expired. If the
    /// intercept parameter is 'true', then that key is not displayed.
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan wait, bool intercept = true)
    {
        var ini = DateTime.UtcNow;
        while (true)
        {
            if (_Console.KeyAvailable) return _Console.ReadKey(intercept);

            var now = DateTime.UtcNow;
            if (now - ini > wait) return null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a formatted message.
    /// </summary>
    static string FormatMessage(this string message, params object?[] args)
    {
        message ??= string.Empty;
        args ??= new object?[] { null };

        return args.Length > 0 ? string.Format(message, args) : message;
    }
}