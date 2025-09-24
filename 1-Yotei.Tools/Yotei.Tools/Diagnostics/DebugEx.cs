namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Debug"/> class.
/// TODO: Use C# 14 static extension methods to extend 'Debug' capabilities.
/// </summary>
public static class DebugEx
{
    static readonly Lock Sync = new();
    static bool DebugAtOrigin = true;

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
    public static void Write(bool console, string? message, params object?[] args)
    {
        lock (Sync)
        {
            message ??= string.Empty;
            args ??= [null];

            if (args.Length > 0) message = string.Format(message, args);

            var manipulate = Ambient.IsConsoleListener();
            var listeners = manipulate ? Ambient.GetConsoleListeners().ToArray() : null;
            if (manipulate) Ambient.RemoveRangeListeners(listeners!);

            var size = Debug.IndentSize;
            var level = Debug.IndentLevel;
            var header = Header(size * level);

            var options = new StringSplitter() { KeepSeparators = true };
            var items = message.Split([Environment.NewLine, "\n"], options);
            foreach (var (value, isseparator) in items)
            {
                if (DebugAtOrigin)
                {
                    if (console) Console.Write(header);
                    Debug.Write(header);
                    DebugAtOrigin = false;
                }
                if (isseparator)
                {
                    if (console) Console.WriteLine();
                    Debug.WriteLine(string.Empty);
                    DebugAtOrigin = true;
                }
                else
                {
                    if (console) Console.Write(value);
                    Debug.Write(value);
                }
            }

            Flush();
            if (manipulate) Trace.Listeners.AddRange(listeners!);
        }
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
    public static void WriteLine(bool console, string? message, params object?[] args)
    {
        lock (Sync)
        {
            message ??= string.Empty;
            args ??= [null];

            if (args.Length > 0) message = string.Format(message, args);

            var manipulate = Ambient.IsConsoleListener();
            var listeners = manipulate ? Ambient.GetConsoleListeners().ToArray() : null;
            if (manipulate) Ambient.RemoveRangeListeners(listeners!);

            var size = Debug.IndentSize;
            var level = Debug.IndentLevel;
            var header = Header(size * level);

            var options = new StringSplitter() { KeepSeparators = true };
            var items = message.Split([Environment.NewLine, "\n"], options);
            foreach (var (value, isseparator) in items)
            {
                if (DebugAtOrigin)
                {
                    if (console) Console.Write(header);
                    Debug.Write(header);
                    DebugAtOrigin = false;
                }
                if (isseparator)
                {
                    if (console) Console.WriteLine();
                    Debug.WriteLine(string.Empty);
                    DebugAtOrigin = true;
                }
                else
                {
                    if (console) Console.Write(value);
                    Debug.Write(value);
                }
            }

            if (console) Console.WriteLine();
            Debug.WriteLine(string.Empty);
            DebugAtOrigin = true;
            Flush();
            if (manipulate) Trace.Listeners.AddRange(listeners!);
        }
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