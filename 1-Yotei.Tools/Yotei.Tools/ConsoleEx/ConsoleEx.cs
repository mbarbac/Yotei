using Yotei.Tools.Diagnostics;

namespace Yotei.Tools.ConsoleEx;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Console"/> class.
/// TODO: Use C# 14 static extension methods to extend 'Console' capabilities.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Clears the console.
    /// </summary>
    public static void Clear() => Console.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        string? message, params object?[] args) => Write(false, message, args);

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, string? message, params object?[] args)
        => Write(false, forecolor, message, args);

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => Write(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);
        Console.Write(message);
        if (debug) DebugEx.Write(false, message);
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line.
    /// </summary>
    public static void WriteLine() => WriteLine(false, string.Empty);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        string? message, params object?[] args) => WriteLine(false, message, args);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, string? message, params object?[] args)
        => WriteLine(false, forecolor, message, args);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => WriteLine(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line and, if requested, to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);
        Console.WriteLine(message);
        if (debug) DebugEx.WriteLine(false, message);
    }

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => ReadLine(false);

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor) => ReadLine(false, forecolor);

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(
        ConsoleColor forecolor, ConsoleColor backcolor) => ReadLine(false, forecolor, backcolor);

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
    /// </summary>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug)
    {
        var str = Console.ReadLine();

        if (str is not null && debug) DebugEx.WriteLine(false, str);
        return str;
    }

    /// <summary>
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next available key from the console, or '<c>null</c>' if no key was available.
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey() => ReadKey(false, false, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Returns the next available key from the console, or '<c>null</c>' if no key was available.
    /// The key is also displayed in the console if such is requested.
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool display) => ReadKey(false, display, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Reads the next available key from the console, waiting for at most the given amount of
    /// time. Returns that key or '<c>null</c>' if no key was available.The key is also displayed
    /// in the console if such is requested.
    /// </summary>
    /// <param name="display"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool display, TimeSpan timeout) => ReadKey(false, display, timeout);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next available key from the console or '<c>null</c>' if no key was available.
    /// The key is also displayed in the console and replicated in the debug environment if such
    /// is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool debug, bool display) => ReadKey(debug, display, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Reads the next available key from the console, waiting for at most the given amount of
    /// time. Returns that key or '<c>null</c>' if no key was available or if the timeout period
    /// expired. The key is also displayed in the console and replicated in the debug environment
    /// if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="display"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool debug, bool display, TimeSpan timeout)
    {
        var ms = timeout.ValidateTimeout();
        var ini = DateTime.UtcNow;

        while (true)
        {
            // There is a key to process...
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(intercept: true);
                if (display)
                {
                    var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                    if (debug) DebugEx.Write(false, ch);
                    Console.Write(ch);
                }
                return info;
            }

            // Let's wait if requested...
            if (ms != -1)
            {
                var now = DateTime.UtcNow;
                if ((now - ini) > timeout) return null;
            }
            Thread.Sleep(10);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or it was cancelled.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, Timeout.InfiniteTimeSpan, source, out result);

    public static bool EditLine(
        ConsoleColor forecolor,
        string? source, [NotNullWhen(true)] out string? result) => throw null;

    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source, [NotNullWhen(true)] out string? result) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled or the timeout expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, timeout, source, out result);

    public static bool EditLine(
        ConsoleColor forecolor,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result) => throw null;

    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled. If accepted, the result is replicated in the
    /// debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(debug, Timeout.InfiniteTimeSpan, source, out result);

    public static bool EditLine(
        ConsoleColor forecolor, bool debug,
        string? source, [NotNullWhen(true)] out string? result) => throw null;

    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, bool debug,
        string? source, [NotNullWhen(true)] out string? result) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled or the timeout expired. If accepted, the result
    /// is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
    {
        throw null;
    }

    public static bool EditLine(
        ConsoleColor forecolor,
        bool debug, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result) => throw null;

    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        bool debug, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result) => throw null;
}