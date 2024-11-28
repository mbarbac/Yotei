namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Console"/> class.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Determines if the messages written to this console class are replicated into the debug
    /// environment, or not, provided there is not registered a console-alike listener. If such,
    /// duplication is ignored.
    /// </summary>
    public static bool DuplicateInDebug { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string? message, params object?[] args) => throw null;

    /// <summary>
    /// Writes the given message, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, using the given foreground and background colors.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line.
    /// </summary>
    public static void WriteLine() => throw null;

    /// <summary>
    /// Writes the given message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string? message, params object?[] args) => throw null;

    /// <summary>
    /// Writes the given message followed by a new line, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message followed by a new line, using the given foreground and background
    /// colors.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null if
    /// no more lines are available.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => throw null;

    /// <summary>
    /// Reads the next available line from the console, using the given foreground color if needed.
    /// Returns either the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = ReadLine();
        Console.ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console, using the given foreground and background
    /// colors if needed. Returns either the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = ReadLine();
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string. Returns whether the edition has been
    /// accepted by the used, or rather it has been cancelled, or the timeout expired. The out
    /// argument contains either the editted string, or null if false.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        string? source,
        TimeSpan timeout, [NotNullWhen(true)] out string? result) => throw null;

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color. Returns
    /// whether the edition has been accepted by the used, or rather it has been cancelled, or the
    /// timeout expired. The out argument contains either the editted string, or null if false.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor,
        string? source,
        TimeSpan timeout, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = EditLine(source, timeout, out result);
        Console.ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors. Returns whether the edition has been accepted by the used, or rather it has been
    /// cancelled, or the timeout expired. The out argument contains either the editted string,
    /// or null if false.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source,
        TimeSpan timeout, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = EditLine(source, timeout, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string. Returns whether the edition has been
    /// accepted by the used or rather it has been cancelled. The out argument contains either
    /// the editted string, or null if false.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        string? source,
        [NotNullWhen(true)] out string? result)
        => EditLine(source, Timeout.InfiniteTimeSpan, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color. Returns
    /// whether the edition has been accepted by the used or rather it has been cancelled. The
    /// out argument contains either the editted string, or null if false.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor,
        string? source,
        [NotNullWhen(true)] out string? result)
        => EditLine(forecolor, source, Timeout.InfiniteTimeSpan, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors. Returns whether the edition has been accepted by the used or rather it has been
    /// cancelled. The out argument contains either the editted string, or null if false.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source,
        [NotNullWhen(true)] out string? result)
        => EditLine(forecolor, backcolor, source, Timeout.InfiniteTimeSpan, out result);

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available pressed key from the console.  The key is also displayed in
    /// the console if such is requested (by default).
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool display = true) => throw null;

    /// <summary>
    /// Reads the next available pressed key from the console, waiting for at most the given
    /// amount of time. Returns either the pressed key or null if the timeout period expired.
    /// The key is also displayed in the console if such is requested (by default).
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool display = true) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Clears the console.
    /// </summary>
    public static void Clear() { }
}