namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Console"/> class.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string? message, params object?[] args) => Write(false, message, args);

    /// <summary>
    /// Writes the given message, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, string? message, params object?[] args)
        => Write(false, forecolor, message, args);

    /// <summary>
    /// Writes the given message, using the given foreground and background colors.
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
    /// Writes the given message, replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        if (debug) DebugEx.Write(true, message, args);
        else
        {
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);
            Console.Write(message);
        }
    }

    /// <summary>
    /// Writes the given message, replicated in the debug environment if such is requested, using
    /// the given foreground color.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, replicated in the debug environment if such is requested, using
    /// the given foreground and background colors.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
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
    /// Writes the given message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string? message, params object?[] args) => WriteLine(false, message, args);

    /// <summary>
    /// Writes the given message followed by a new line, using the given foreground color.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, string? message, params object?[] args)
        => WriteLine(false, forecolor, message, args);

    /// <summary>
    /// Writes the given message followed by a new line, using the given foreground and background
    /// colors.
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
    /// Writes a new line, replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

    /// <summary>
    /// Writes the given message, replicated in the debug environment if such is requested, followed
    /// by a new line.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, string? message, params object?[] args)
    {
        if (message is null) return;

        if (debug) DebugEx.WriteLine(true, message, args);
        else
        {
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// Writes the given message, replicated in the debug environment if such is requested, followed
    /// by a new line, using the given foreground color.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, replicated in the debug environment if such is requested, followed
    /// by a new line, using the given foreground and background colors.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => ReadLine(false);

    /// <summary>
    /// Reads the next available line from the console, using the given foreground color if
    /// needed. Returns either the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor) => ReadLine(false, forecolor);

    /// <summary>
    /// Reads the next available line from the console, using the given foreground and background
    /// colors if needed. Returns either the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(
        ConsoleColor forecolor, ConsoleColor backcolor) => ReadLine(false, forecolor, backcolor);

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
    /// </summary>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug)
    {
        var r = Console.ReadLine();

        if (r is not null && debug)
        {
            // We need to manipulate the collection of listerners because otherwise the message
            // might be duplicated in the console...

            var found = Trace.Listeners.Any(x =>
                x is TextWriterTraceListener temp &&
                ReferenceEquals(Console.Out, temp.Writer));

            var items = !found ? [] : Ambient.UnregisterConsoleListeners();
            Debug.WriteLine(r);
            Ambient.RegisterConsoleListeners(items);
        }
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console, using the given foreground color if
    /// needed, and replicates the result in the debug environment if requested. Returns either
    /// the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console, using the given foreground and background
    /// colors if needed, and replicates the result in the debug environment if requested. Returns
    /// either the read string, or null if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = ReadLine(debug);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted the
    /// edition, or if it was cancelled or the timeout expired. The out argument contains the editted
    /// result, or null.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, timeout, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color, returning
    /// whether the user has accepted the edition, or if it was cancelled or the timeout expired.
    /// The out argument contains the editted result, or null.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, forecolor, timeout, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors, returning whether the user has accepted the edition, or if it was cancelled or
    /// the timeout expired. The out argument contains the editted result, or null.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, forecolor, backcolor, timeout, source, out result);

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted the
    /// edition, or if it was cancelled. The out argument contains the editted result, or null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color, returning
    /// whether the user has accepted the edition, or if it was cancelled. The out argument contains
    /// the editted result, or null.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, forecolor, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors, returning whether the user has accepted the edition, or if it was cancelled. The
    /// out argument contains the editted result, or null.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, forecolor, backcolor, source, out result);

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted the
    /// edition, or if it was cancelled or the timeout expired. The out argument contains the editted
    /// result, or null, which is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var size = Console.CursorSize;
        var left = Console.CursorLeft;
        var insert = false;
        int len;

        var sb = new StringBuilder(source ?? string.Empty);
        var pos = sb.Length;
        ShowLine(pos);

        // Main loop...
        while (true)
        {
            // This resets the timeout period for each new key...
            var info = ReadKey(timeout, false);
            info ??= new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            // Special keys...
            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    SetInsert(false);
                    Console.WriteLine();
                    result = sb.ToString();

                    if (debug)
                    {
                        var found = Trace.Listeners.Any(x =>
                            x is TextWriterTraceListener temp &&
                            ReferenceEquals(Console.Out, temp.Writer));

                        var items = !found ? [] : Ambient.UnregisterConsoleListeners();
                        Debug.WriteLine(result);
                        Ambient.RegisterConsoleListeners(items);
                    }

                    return true;

                case ConsoleKey.Escape:
                    SetInsert(false);
                    len = sb.Length; sb.Clear();
                    ShowLine(0, len);
                    Console.WriteLine();
                    result = null;
                    return false;

                case ConsoleKey.Insert:
                    SetInsert(!insert);
                    break;

                case ConsoleKey.Home:
                    pos = 0;
                    Console.CursorLeft = left;
                    break;

                case ConsoleKey.End:
                    pos = sb.Length;
                    Console.CursorLeft = left + pos;
                    break;

                case ConsoleKey.Delete:
                    if (pos < sb.Length)
                    {
                        len = sb.Length; sb.Remove(pos, 1);
                        ShowLine(pos, len);
                    }
                    break;

                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        len = sb.Length; sb.Remove(--pos, 1);
                        ShowLine(pos, len);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (pos > 0)
                    {
                        if (!info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            pos--;
                            Console.CursorLeft--;
                            break;
                        }
                        else
                        {
                            var ascii = char.IsLetterOrDigit(sb[pos - 1]);
                            while (pos > 0)
                            {
                                var temp = char.IsLetterOrDigit(sb[pos - 1]);
                                if (temp == ascii)
                                {
                                    pos--;
                                    Console.CursorLeft--;
                                }
                                else break;
                            }
                        }
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (pos < sb.Length)
                    {
                        if (!info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            pos++;
                            Console.CursorLeft++;
                            break;
                        }
                        else
                        {
                            var ascii = char.IsLetterOrDigit(sb[pos]);
                            while (pos < sb.Length)
                            {
                                var temp = char.IsLetterOrDigit(sb[pos]);
                                if (temp == ascii)
                                {
                                    pos++;
                                    Console.CursorLeft++;
                                }
                                else break;
                            }
                        }
                    }
                    break;
            }

            // Regular keys...
            if (info.Value.KeyChar >= 32)
            {
                if (insert)
                {
                    sb.Insert(pos, info.Value.KeyChar);
                    ShowLine(++pos);
                }
                else
                {
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
            }
        }

        // Sets the cursor to insert mode, or not...
        void SetInsert(bool value)
        {
            if (value == insert) return;

            var temp = value ? 25 : size;
            var iswindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (iswindows) Console.CursorSize = temp;
            insert = value;
        }

        // Shows the current value, sets cursor position, and clear remaining len...
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
    /// Edits in the console the given source string, using the given foreground color, returning
    /// whether the user has accepted the edition, or if it was cancelled or the timeout expired.
    /// The out argument contains the editted result, or null, which is replicated in the debug
    /// environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, ConsoleColor forecolor, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = EditLine(debug, timeout, source, out result);
        Console.ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors, returning whether the user has accepted the edition, or if it was cancelled or
    /// the timeout expired. The out argument contains the editted result, or null, which is
    /// replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = EditLine(debug, timeout, source, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted the
    /// edition, or if it was cancelled or the timeout expired. The out argument contains the editted
    /// result, or null, which is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(debug, Timeout.InfiniteTimeSpan, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color, returning
    /// whether the user has accepted the edition, or if it was cancelled or the timeout expired.
    /// The out argument contains the editted result, or null, which is replicated in the debug
    /// environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, ConsoleColor forecolor,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(debug, forecolor, Timeout.InfiniteTimeSpan, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors, returning whether the user has accepted the edition, or if it was cancelled or
    /// the timeout expired. The out argument contains the editted result, or null, which is
    /// replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor,
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(debug, forecolor, backcolor, Timeout.InfiniteTimeSpan, source, out result);

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available pressed key from the console and displays it in the console.
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey() => ReadKey(true);

    /// <summary>
    /// Reads the next available pressed key from the console. The key is also displayed in the
    /// console if requested.
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool display) => ReadKey(false, Timeout.InfiniteTimeSpan, display);

    /// <summary>
    /// Reads the next available pressed key from the console, waiting for at most the given
    /// amount of time. Returns either the pressed key or null if the timeout period expired.
    /// The key is also displayed in the console if requested.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        TimeSpan timeout, bool display) => ReadKey(false, timeout, display);

    /// <summary>
    /// Reads the next available pressed key from the console. The key is also displayed in the
    /// console, and replicated in the debug environment, if such things are requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool debug, bool display) => ReadKey(debug, Timeout.InfiniteTimeSpan, display);

    /// <summary>
    /// Reads the next available pressed key from the console, waiting for at most the given
    /// amount of time. Returns either the pressed key or null if the timeout period expired.
    /// The key is also displayed in the console, and replicated in the debug environment, if
    /// such things are requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool debug, TimeSpan timeout, bool display)
    {
        var ms = timeout.ValidateTimeout();
        var ini = DateTime.UtcNow;

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(intercept: true);
                var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                if (display) Console.Write(ch);

                if (debug)
                {
                    var found = Trace.Listeners.Any(x =>
                        x is TextWriterTraceListener temp &&
                        ReferenceEquals(Console.Out, temp.Writer));

                    var items = !found ? [] : Ambient.UnregisterConsoleListeners();
                    Debug.Write(ch);
                    Ambient.RegisterConsoleListeners(items);
                }

                return info;
            }

            if (ms != -1)
            {
                var now = DateTime.UtcNow;
                if ((now - ini) > timeout) return null;
            }

            Thread.Sleep(10);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Clears the console.
    /// </summary>
    public static void Clear() => Console.Clear();
}