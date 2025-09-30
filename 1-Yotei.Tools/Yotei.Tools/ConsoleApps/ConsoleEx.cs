namespace Yotei.Tools;

// =============================================================
/// <summary>
/// Provides a <see cref="Console"/> replacement with extended functionalities.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Executes the given action in an ambient with no console listeners.
    /// </summary>
    /// <param name="action"></param>
    public static void WithNoConsoleListeners(Action action)
    {
        action.ThrowWhenNull();

        lock (Ambient.Lock)
        {
            var items = Ambient.GetConsoleListeners().ToArray();
            Ambient.RemoveListeners(items);
            try { action(); }
            finally { Ambient.AddListeners(items); }
        }
    }

    // ---------------------------------------------------------

    /// <inheritdoc cref="Console.Clear"/>
    public static void Clear() => Console.Clear();

    // ---------------------------------------------------------

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        string message, params object?[]? args) => Write(false, message, args);

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground and background colors.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, and replicates it
    /// to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Console.Write(message);
        if (debug) WithNoConsoleListeners(() => Debug.Write(message));
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground color, and replicates it to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug, ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground and background colors, and replicates it to the not console-alike debug
    /// outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Writes a line terminator.
    /// </summary>
    public static void WriteLine() => WriteLine(false);

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        string message, params object?[]? args) => WriteLine(false, message, args);

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, using the given
    /// foreground and background colors.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor,
        ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Writes a line terminator and replicates it to the not console-alike debug outputs if
    /// requested.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, and replicates it to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Console.WriteLine(message);
        if (debug) WithNoConsoleListeners(() => Debug.WriteLine(message));
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, using the given foreground color, and replicates it to the not console-alike
    /// debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, formatted with the given arguments, if any, followed by a line
    /// terminator, using the given foreground and background colors, and replicates it to the
    /// not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor,
        ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Returns the next line of characters read from the console, or null if no characters are
    /// available.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => Console.ReadLine();

    /// <summary>
    /// <inheritdoc cref="ReadLine()"/> Replicates the result to the not console-alike debug
    /// outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug)
    {
        var str = ReadLine();

        if (str is not null && debug) WithNoConsoleListeners(() => Debug.WriteLine(str));
        return str;
    }

    /// <summary>
    /// <inheritdoc cref="ReadLine()"/> Uses the given foreground color. Replicates the result to
    /// the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var result = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        return result;
    }

    /// <summary>
    /// <inheritdoc cref="ReadLine()"/> Uses the given foreground and background colors. Replicates
    /// the result to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var result = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return result;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Waits to read from the console the next character or function key pressed by the user
    /// and displays it.
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey() => Console.ReadKey();

    /// <summary>
    /// Waits to read from the console the next character or function key pressed by the user
    /// and displays it if '<paramref name="intercept"/>' is false.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

    /// <summary>
    /// Waits to read from the console the next character or function key pressed by the user
    /// and displays it if '<paramref name="intercept"/>' is false. Replicates the result to
    /// the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept, bool debug)
    {
        var info = ReadKey(intercept, debug, Timeout.InfiniteTimeSpan);
        return (ConsoleKeyInfo)info!;
    }

    /// <summary>
    /// Tries to read from the console the next character or function key pressed by the user
    /// and returns it, or null if the timeout expired. If any was read, it is displayed.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        TimeSpan timeout) => ReadKey(false, false, timeout);

    /// <summary>
    /// Tries to read from the console the next character or function key pressed by the user
    /// and returns it, or null if the timeout expired. If any was read, it is displayed.
    /// Replicates the result to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool intercept, TimeSpan timeout) => ReadKey(intercept, false, timeout);

    /// <summary>
    /// Tries to read from the console the next character or function key pressed by the user
    /// and returns it, or null if the timeout expired. If any was read, it is displayed if
    /// '<paramref name="intercept"/>' is false. Replicates the result to the not console-alike
    /// debug outputs if requested.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool intercept, bool debug, TimeSpan timeout)
    {
        var ms = timeout.ValidatedTimeout;
        var ini = DateTime.UtcNow;

        while (true)
        {
            // Key is available...
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(intercept: true);
                if (!intercept)
                {
                    var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                    Console.Write(ch);
                    if (debug) WithNoConsoleListeners(() => Debug.Write(ch));
                }
                return info;
            }

            // Waiting...
            if (ms > -1)
            {
                var now = DateTime.UtcNow;
                var span = now - ini;
                if (span >= timeout) return null;
            }
            Thread.Sleep(1);
        }
    }
}