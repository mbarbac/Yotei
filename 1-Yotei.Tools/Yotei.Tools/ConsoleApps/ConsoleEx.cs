using System.Reflection.Metadata.Ecma335;

namespace Yotei.Tools;

// ========================================================
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

    // ----------------------------------------------------

    /// <inheritdoc cref="Console.Clear"/>
    public static void Clear() => Console.Clear();

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    // It bothers me that the first parameter of the standard 'ReadKey(...)' method is a boolean
    // one ('intercepts') because when 'debug' is used in all 'ConsoleEx' methods it appears as
    // the first one. As the idea is that 'ConsoleEx' mimics the standard APIs we have very no
    // that much room to manouver: we've decided to include 'debug' as the 2nd parameter after
    // the 1st 'intercept' one. If only one boolean is used, it is a pitty, but it is the standard
    // intercept one.

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

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired. Replicates the result to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(bool debug, TimeSpan timeout, string? source)
    {
        var size = Console.CursorSize;
        var left = Console.CursorLeft;
        var insert = false;

        var sb = new StringBuilder(); sb.Append(source ?? string.Empty);
        var pos = sb.Length;
        int len;

        SetInsert(false);
        ShowLine(pos);
        while (true)
        {
            var info = ReadKey(true, false, timeout);
            info ??= new('\0', ConsoleKey.Escape, false, false, false);

            // Special keys...
            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    SetInsert(false);
                    Console.WriteLine();
                    if (debug) WithNoConsoleListeners(() => Debug.WriteLine(sb.ToString()));
                    return sb.ToString();

                case ConsoleKey.Escape:
                    SetInsert(false);
                    len = sb.Length; sb.Clear(); ShowLine(0, len);
                    Console.WriteLine();
                    return null;

                case ConsoleKey.Insert:
                    SetInsert(!insert);
                    break;

                case ConsoleKey.Home:
                    pos = 0;
                    Console.CursorLeft = left + pos;
                    break;

                case ConsoleKey.End:
                    pos = sb.Length;
                    Console.CursorLeft = left + pos;
                    break;

                case ConsoleKey.Delete:
                    if (pos >= sb.Length) break;
                    len = sb.Length;
                    sb.Remove(pos, 1);
                    ShowLine(pos, len);
                    break;

                case ConsoleKey.Backspace:
                    if (pos == 0) break;
                    len = sb.Length;
                    sb.Remove(--pos, 1);
                    ShowLine(pos, len);
                    break;

                case ConsoleKey.LeftArrow:
                    if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        if (pos == 0) break;
                        var ascii = char.IsLetterOrDigit(sb[pos - 1]);
                        while (pos > 0)
                        {
                            var temp = char.IsLetterOrDigit(sb[pos - 1]);
                            if (temp == ascii) { pos--; Console.CursorLeft--; }
                            else break;
                        }
                    }
                    else
                    {
                        if (pos > 0) { pos--; Console.CursorLeft--; }
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        if (pos >= sb.Length) break;
                        var ascii = char.IsLetterOrDigit(sb[pos]);
                        while (pos < sb.Length)
                        {
                            var temp = char.IsLetterOrDigit(sb[pos]);
                            if (temp == ascii) { pos++; Console.CursorLeft++; }
                            else break;
                        }
                    }
                    else
                    {
                        if (pos < sb.Length) { pos++; Console.CursorLeft++; }
                    }
                    break;
            }

            // Standard keys...
            if (info.Value.KeyChar < ' ') continue;

            if (insert)
            {
                sb.Insert(pos, info.Value.KeyChar);
                ShowLine(++pos);
                continue;
            }

            if (pos < sb.Length)
            {
                sb[pos] = info.Value.KeyChar;
                ShowLine(++pos);
            }
            else
            {
                sb.Append(info.Value.KeyChar);
                Console.Write(info.Value.KeyChar);
                pos++;
            }
        }

        /// <summary>
        /// Sets the insert mode to ON (true) of OFF (false).
        /// </summary>
        void SetInsert(bool value)
        {
            var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (windows) Console.CursorSize = value ? 100 : size;
            insert = value;
        }

        /// <summary>
        /// Shows the current value, clears up to remaining len, and sets cursos position.
        /// </summary>
        void ShowLine(int pos, int len = 0)
        {
            Console.CursorLeft = left;
            Console.Write(sb);

            len -= sb.Length;
            if (len > 0) Console.Write(DebugEx.Header(len));
            Console.CursorLeft = left + pos;
        }
    }

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired. Replicates the result to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, ConsoleColor forecolor, TimeSpan timeout, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var result = EditLine(debug, timeout, source);
        Console.ForegroundColor = oldfore;
        return result;
    }

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired. Replicates the result to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var result = EditLine(debug, timeout, source);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return result;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        TimeSpan timeout, string? source) => EditLine(false, timeout, source);

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine( ConsoleColor forecolor, TimeSpan timeout, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var result = EditLine(false, timeout, source);
        Console.ForegroundColor = oldfore;
        return result;
    }

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled or if the timeout has
    /// expired.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var result = EditLine(false, timeout, source);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return result;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled. Replicates the result
    /// to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, string? source) => EditLine(debug, Timeout.InfiniteTimeSpan, source);

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled. Replicates the result
    /// to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine( bool debug, ConsoleColor forecolor, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var result = EditLine(debug, Timeout.InfiniteTimeSpan, source);
        Console.ForegroundColor = oldfore;
        return result;
    }

    /// <summary>
    /// Edits in the console the given source string (null ones are translated into empty ones),
    /// and returns the result of that editing, or null if it was cancelled. Replicates the result
    /// to the not console-alike debug outputs if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor, string? source)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var result = EditLine(debug, Timeout.InfiniteTimeSpan, source);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return result;
    }
}