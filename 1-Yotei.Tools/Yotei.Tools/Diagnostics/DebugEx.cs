namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Debug"/> class.
/// </summary>
public static class DebugEx
{
    /// <summary>
    /// Determines if the messages written to this debug class are replicated into the console
    /// environment, or not, provided there is not registered a console-alike listener. If such,
    /// duplication is ignored.
    /// </summary>
    public static bool DuplicateInConsole { get; set; }

    /// <inheritdoc cref="Debug.IndentSize"/>
    public static int IndentSize
    {
        get => Debug.IndentSize;
        set => Debug.IndentSize = value;
    }

    /// <inheritdoc cref="Debug.IndentLevel"/>
    public static int IndentLevel
    {
        get => Debug.IndentLevel;
        set => Debug.IndentLevel = value;
    }

    /// <inheritdoc cref="Debug.AutoFlush"/>
    public static bool AutoFlush
    {
        get => Debug.AutoFlush;
        set => Debug.AutoFlush = value;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="Debug.Flush"/>
    [Conditional("DEBUG")]
    public static void Flush() => Debug.Flush();

    /// <inheritdoc cref="Debug.Indent"/>
    [Conditional("DEBUG")]
    public static void Indent() => Debug.Indent();

    /// <inheritdoc cref="Debug.Unindent"/>
    [Conditional("DEBUG")]
    public static void Unindent() => Debug.Unindent();

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(string? message, params object?[] args) => throw null;

    /// <summary>
    /// Writes the given message, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
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
    [Conditional("DEBUG")]
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
    [Conditional("DEBUG")]
    public static void WriteLine() => throw null;

    /// <summary>
    /// Writes the given message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(string? message, params object?[] args) => throw null;

    /// <summary>
    /// Writes the given message followed by a new line, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
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
    [Conditional("DEBUG")]
    public static void WriteLine(ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }
}