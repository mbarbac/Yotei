namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Debug"/> class.
/// TODO: Use C# 14 static extension methods to extend 'Debug' capabilities.
/// </summary>
public static class DebugEx
{
    static readonly Lock Sync = new();

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        string? message, params object?[] args) => Write(false, message, args);

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
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
    [Conditional("DEBUG")]
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => Write(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write( bool console, string? message, params object?[] args)
    {
        throw null;
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        bool console, ConsoleColor forecolor, string? message, params object?[] args)
    {
        lock (Sync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Write(console, message, args);
            Console.ForegroundColor = oldfore;
        }
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        bool console,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        lock (Sync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Write(console, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine() => WriteLine(false);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        string? message, params object?[] args) => WriteLine(false, message, args);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
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
    [Conditional("DEBUG")]
    public static void WriteLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => WriteLine(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console) => WriteLine(console, string.Empty);

    /// <summary>
    /// Writes a new line and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine( bool console, string? message, params object?[] args)
    {
        throw null;
    }

    /// <summary>
    /// Writes a new line and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        bool console, ConsoleColor forecolor, string? message, params object?[] args)
    {
        lock (Sync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteLine(console, message, args);
            Console.ForegroundColor = oldfore;
        }
    }

    /// <summary>
    /// Writes a new line and, if requested, replicates it to the console.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        bool console,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        lock (Sync)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteLine(console, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }
    }
}