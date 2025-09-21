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
}