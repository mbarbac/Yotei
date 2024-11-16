namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Debug"/> class.
/// </summary>
public static class DebugEx
{
    private const bool DEFAULTCONSOLE = false;

    /// <summary>
    /// Determines if, in the DEBUG environment, we are at the origin of a new line, or not.
    /// </summary>
    public static bool DebugAtOrigin { get; internal set; } = true;

    // ----------------------------------------------------

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
    /// Writes the given debug message to the registered listeners.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(string? message, params object?[] args)
    {
        Write(DEFAULTCONSOLE, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, and to the console if no
    /// console listener is registered, if such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, string? message, params object?[] args)
    {
        Write(console, Console.ForegroundColor, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, using the given color,
    /// if such is needed.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(ConsoleColor color, string? message, params object?[] args)
    {
        Write(DEFAULTCONSOLE, color, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, and to the console if no
    /// console listener is registered using the given color, if such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, ConsoleColor color, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;

        if (message == null) goto WRITEEND;
        if (message.Length == 0) goto WRITEEND;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        if (console && !Ambient.IsConsoleListener())
        {
            var size = Debug.IndentSize;
            var level = Debug.IndentLevel;
            var header = Header(size * level);

            var iter = new SpanCharSplitter(message, "\r\n", "\n");
            foreach (var item in iter)
            {
                var part = item.ToString();
                var temp = DebugAtOrigin ? part : $"{header}{part}";
                Console.Write(temp);

                DebugAtOrigin =
                    iter.CurrentIsSeparator ||
                    (part.Length == 0 && DebugAtOrigin);
            }
        }

        Debug.Write(message);

        WRITEEND:
        Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line to the registered listeners.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine() => WriteLine("");

    /// <summary>
    /// Writes a new line to the registered listeners, and to the console if no console listener
    /// is registered, and if such is requested.
    /// </summary>
    /// <param name="console"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console) => WriteLine(console, "");

    /// <summary>
    /// Writes the given debug message to the registered listeners, and then a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(string? message, params object?[] args)
    {
        WriteLine(DEFAULTCONSOLE, Console.ForegroundColor, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners followed by a new line, and to
    /// the console if no console listener is registered, and if such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console, string? message, params object?[] args)
    {
        WriteLine(console, Console.ForegroundColor, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners followed by a new line, using
    /// the given color, if such is needed.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(ConsoleColor color, string? message, params object?[] args)
    {
        Write(DEFAULTCONSOLE, color, message, args);
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners followed by a new line, and
    /// to the console if no console listener is registered, using the given color, if such is
    /// requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console, ConsoleColor color, string? message, params object?[] args)
    {
        Write(console, color, message, args);

        if (console && !Ambient.IsConsoleListener()) Console.WriteLine(string.Empty);
        Debug.WriteLine(string.Empty);
        DebugAtOrigin = true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
    internal static string Header(int size)
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

        if (!Headers.TryGetValue(size, out var header)) Headers.Add(size, header = new(' ', size));
        return header;
    }
    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(' ', 1);
    readonly static string Header2 = new(' ', 2);
    readonly static string Header3 = new(' ', 3);
    readonly static string Header4 = new(' ', 4);
    readonly static string Header5 = new(' ', 5);
    readonly static string Header6 = new(' ', 6);
    readonly static string Header7 = new(' ', 7);
    readonly static string Header8 = new(' ', 8);
    readonly static string Header9 = new(' ', 9);
    readonly static Dictionary<int, string> Headers = [];
}