namespace Runner;

// ========================================================
/// <summary>
/// Represents a replacement or wrapper over the <see cref="Debug"/> one.
/// </summary>
public static class DebugEx
{
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
        throw null;
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, and to the console if no
    /// console listener is registered, and such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, string? message, params object?[] args)
    {
        throw null;
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, and to the console if no
    /// console listener is registered using the given color, and such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, ConsoleColor color, string? message, params object?[] args)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line to the registered listeners.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine()
    {
        throw null;
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners, and then a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(string? message, params object?[] args)
    {
        throw null;
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
        throw null;
    }

    /// <summary>
    /// Writes the given debug message to the registered listeners followed by a new line, and to
    /// the console if no console listener is registered, using the given color, and if such is
    /// requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console, ConsoleColor color, string? message, params object?[] args)
    {
        throw null;
    }
}