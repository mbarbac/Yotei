namespace Yotei.Tools.Diagnostics;

// =============================================================
/// <summary>
/// Provides a <see cref="Debug"/> replacement with extensions for console apps.
/// </summary>
public static class DebugEx
{
    /// <inheritdoc cref="Debug.AutoFlush"/>
    public static bool AutoFlush
    {
        get => Debug.AutoFlush;
        set => Debug.AutoFlush = value;
    }

    /// <inheritdoc cref="Debug.IndentLevel"/>"/>
    public static int IndentLevel
    {
        get => Debug.IndentLevel;
        set => Debug.IndentLevel = value;
    }

    /// <inheritdoc cref="Debug.IndentSize"/>"/>
    public static int IndentSize
    {
        get => Debug.IndentSize;
        set => Debug.IndentSize = value;
    }

    // ---------------------------------------------------------

    /// <inheritdoc cref="Debug.Flush"/>
    public static void Flush() => Debug.Flush();

    /// <inheritdoc cref="Debug.Indent"/>/>
    public static void Indent() => Debug.Indent();

    /// <inheritdoc cref="Debug.Unindent"/>/>
    public static void Unindent() => Debug.Unindent();

    // ---------------------------------------------------------

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, and replicates it
    /// to the console output if requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        bool console,
        string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Debug.Write(message);
        if (console && !Ambient.IsConsoleListener())
        {
            WriteConsoleHeader();
            Console.Write(message);
        }
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground color, and replicates it to the console output if requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        bool console, ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(console, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground and background colors, and replicates it to the console output if requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(
        bool console,
        ConsoleColor forecolor, ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(console, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Writes a line terminator and replicates it to the console output if requested.
    /// </summary>
    /// <param name="console"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console) => WriteLine(console, string.Empty);

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, and replicates it to the console output if requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        bool console,
        string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Debug.WriteLine(message);
        if (console && !Ambient.IsConsoleListener())
        {
            WriteConsoleHeader();
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, using the given foreground color, and replicates it to the console output if
    /// requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        bool console,
        ConsoleColor forecolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(console, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, using the given foreground and background colors, and replicates it to the
    /// console output if requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(
        bool console,
        ConsoleColor forecolor,
        ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(console, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Invoked to write a console header.
    /// </summary>
    static void WriteConsoleHeader()
    {
        var size = Debug.IndentSize * Debug.IndentLevel;
        var header = size == 0 ? string.Empty : Header(size);
        if (size > 0) Console.Write(header);
    }

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
    static string Header(int size)
    {
        switch (size)
        {
            case 0: return Header0;
            case 1: return Header1;
            case 2: return Header2;
            case 3: return Header3;
            case 4: return Header4;
            case 5: return Header5;
            case 6: return Header6;
            case 7: return Header7;
            case 8: return Header8;
            case 9: return Header9;
        }

        if (!Headers.TryGetValue(size, out var header)) Headers.Add(size, header = new(Space, size));
        return header;
    }

    readonly static Dictionary<int, string> Headers = [];
    readonly static char Space = ' ';

    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(Space, 1);
    readonly static string Header2 = new(Space, 2);
    readonly static string Header3 = new(Space, 3);
    readonly static string Header4 = new(Space, 4);
    readonly static string Header5 = new(Space, 5);
    readonly static string Header6 = new(Space, 6);
    readonly static string Header7 = new(Space, 7);
    readonly static string Header8 = new(Space, 8);
    readonly static string Header9 = new(Space, 9);
}