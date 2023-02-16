namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a wrapper over the <see cref="_Debug"/> class.
/// </summary>
public static class DebugWrapper
{
    /// <summary>
    /// Invoked to obtain a formatted message.
    /// </summary>
    static string FormatMessage(this string message, params object?[] args)
    {
        message ??= string.Empty;
        args ??= new object?[] { null };

        return args.Length > 0 ? string.Format(message, args) : message;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The indentation size.
    /// </summary>
    public static int IndentSize
    {
        get => _Debug.IndentSize;
        set => _Debug.IndentSize = value;
    }

    /// <summary>
    /// The indentation level.
    /// </summary>
    public static int IndentLevel
    {
        get => _Debug.IndentLevel;
        set => _Debug.IndentLevel = value;
    }

    /// <summary>
    /// Whether to auto-flush _Debug messages or not.
    /// </summary>
    public static bool AutoFlush
    {
        get => _Debug.AutoFlush;
        set => _Debug.AutoFlush = value;
    }

    /// <summary>
    /// Flusg _Debug messages.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Flush() => _Debug.Flush();

    /// <summary>
    /// Increases the indentation.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Indent() => _Debug.Indent();

    /// <summary>
    /// Decreases the indentation
    /// </summary>
    [Conditional("DEBUG")]
    public static void Unindent() => _Debug.Unindent();

    // ----------------------------------------------------

    /// <summary>
    /// Writes a formatted message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        _Debug.Write(message);
        if (!Ambient.IsDebugOnConsole()) _Console.Write(message);
    }

    /// <summary>
    /// Writes a formatted message
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        Write(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine() => WriteLine(string.Empty);

    /// <summary>
    /// Writes a formatted message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        _Debug.WriteLine(message);
        if (!Ambient.IsDebugOnConsole()) _Console.WriteLine(message);
    }

    /// <summary>
    /// Writes a formatted message followed by a new line.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        WriteLine(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Increases indentation and writes a formatted message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void IndentWrite(string message, params object?[] args)
    {
        Indent();
        Write(message, args);
    }

    /// <summary>
    /// Increases indentation and writes a formatted message
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void IndentWrite(Color color, string message, params object?[] args)
    {
        Indent();
        Write(color, message, args);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Increases indentation and writes a formatted message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void IndentWriteLine(string message, params object?[] args)
    {
        Indent();
        WriteLine(message, args);
    }

    /// <summary>
    /// Increases indentation and writes a formatted message followed by a new line.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void IndentWriteLine(Color color, string message, params object?[] args)
    {
        Indent();
        WriteLine(color, message, args);
    }
}