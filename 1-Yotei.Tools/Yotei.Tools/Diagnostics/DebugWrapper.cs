using Color = System.ConsoleColor;

namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents a wrapper over the <see cref="Debug"/> class.
/// </summary>
public static class DebugWrapper
{
    private const bool DEFAULTCONSOLE = true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Debug.IndentSize"/>
    /// </summary>
    public static int IndentSize
    {
        get => Debug.IndentSize;
        set => Debug.IndentSize = value;
    }

    /// <summary>
    /// <inheritdoc cref="Debug.IndentLevel"/>
    /// </summary>
    public static int IndentLevel
    {
        get => Debug.IndentLevel;
        set => Debug.IndentLevel = value;
    }

    /// <summary>
    /// <inheritdoc cref="Debug.AutoFlush"/>
    /// </summary>
    public static bool AutoFlush
    {
        get => Debug.AutoFlush;
        set => Debug.AutoFlush = value;
    }

    /// <summary>
    /// <inheritdoc cref="Debug.Flush"/>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Flush() => Debug.Flush();

    /// <summary>
    /// <inheritdoc cref="Debug.Indent"/>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Indent() => Debug.Indent();

    /// <summary>
    /// <inheritdoc cref="Debug.Unindent"/>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Unindent() => Debug.Unindent();

    // ----------------------------------------------------

    /// <summary>
    /// Writes in the debug listeners an empty message, and replicates it in the console.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Write() { }

    /// <summary>
    /// Writes in the debug listeners an empty message, and replicates it in the console if such
    /// is requested.
    /// </summary>
    /// <param name="console"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console) { }

    /// <summary>
    /// Writes in the debug listeners a formatted message, and replicates it in the console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(string? message, params object?[] args)
    {
        Write(DEFAULTCONSOLE, message, args);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message, and replicates it in the console if
    /// such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= Wrappers.NullArray;
        if (args.Length > 0) message = string.Format(message, args);

        if (console && !Ambient.IsConsoleListener())
        {
            var size = Debug.IndentSize;
            var level = Debug.IndentLevel;
            var header = Wrappers.Header(size * level);

            var parts = Wrappers.TokenizeNL(message);
            foreach (var part in parts)
            {
                var item = Wrappers.DebugAtOrigin switch
                {
                    true => $"{header}{part}",
                    _ => part
                };
                Console.Write(item);

                if (part.EndsWith('\n') ||
                    part.EndsWith(Environment.NewLine))
                    Wrappers.DebugAtOrigin = true;
            }
        }

        Debug.Write(message);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message using the given color, and replicates
    /// it in the console.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(Color color, string? message, params object?[] args)
    {
        Write(DEFAULTCONSOLE, color, message, args);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message, using the given color, and replicates
    /// it in the console, if such is requested.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void Write(bool console, Color color, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        Write(console, message, args);
        Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes in the debug listeners an empty message followed by a new line, and replicates it
    /// in the console.
    /// </summary>
    [Conditional("DEBUG")]
    public static void WriteLine() => WriteLine(DEFAULTCONSOLE);

    /// <summary>
    /// Writes in the debug listeners an empty message followed by a new line, and replicates it
    /// in the console if such is requested.
    /// </summary>
    /// <param name="console"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console) => WriteLine(console, string.Empty);

    /// <summary>
    /// Writes in the debug listeners a formatted message followed by a new line, and replicates
    /// it in the console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(string? message, params object?[] args)
    {
        WriteLine(DEFAULTCONSOLE, message, args);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message followed by a new line, and replicates
    /// it in the console if such is requested.
    /// </summary>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console, string? message, params object?[] args)
    {
        if (message is null) return;

        if (message.Length != 0)
        {
            args ??= Wrappers.NullArray;
            if (args.Length > 0) message = string.Format(message, args);
        }

        if (console && !Ambient.IsConsoleListener())
        {
            var size = Debug.IndentSize;
            var level = Debug.IndentLevel;
            var header = Wrappers.Header(size * level);

            var parts = Wrappers.TokenizeNL(message);
            foreach (var part in parts)
            {
                var item = Wrappers.DebugAtOrigin switch
                {
                    true => $"{header}{part}",
                    _ => part
                };
                Console.WriteLine(item);

                if (part.EndsWith('\n') ||
                    part.EndsWith(Environment.NewLine))
                    Wrappers.DebugAtOrigin = true;
            }
        }

        Debug.WriteLine(message);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message followed by a new line, using the given
    /// color, and replicates it in the console.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(Color color, string? message, params object?[] args)
    {
        WriteLine(DEFAULTCONSOLE, color, message, args);
    }

    /// <summary>
    /// Writes in the debug listeners a formatted message followed by a new line, using the given
    /// color, and replicates it in the console if such is requested.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="console"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [Conditional("DEBUG")]
    public static void WriteLine(bool console, Color color, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        WriteLine(console, message, args);
        Console.ForegroundColor = old;
    }
}