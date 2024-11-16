namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Console"/> class.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified. If the message is null, then the parameters are
    /// ignored.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Console.Write(message);
    }

    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified, using the given foreground color. If the message is
    /// null, then the parameters are ignored.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(ConsoleColor forecolor, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(message, args);
        Console.ForegroundColor = old;
    }

    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified, using the given foreground and background colors. If
    /// the message is null, then the parameters are ignored.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        var oldfg = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldbk = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        Write(message, args);
        Console.ForegroundColor = oldfg;
        Console.BackgroundColor = oldbk;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes in the console a new line.
    /// </summary>
    public static void WriteLine() => Console.WriteLine();

    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified, and then a new line. If the message is null, then
    /// the parameters are ignored.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string? message, params object?[] args)
    {
        Write(message, args);
        WriteLine();
    }

    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified, and then a new line, using the given foreground color.
    /// If the message is null, then the parameters are ignored.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(ConsoleColor forecolor, string? message, params object?[] args)
    {
        Write(forecolor, message, args);
        WriteLine();
    }

    /// <summary>
    /// Writes in the console the given message, which is used as the format specification in
    /// case parameters are also specified, and then a new line, using the given foreground and
    /// background colors. If the message is null, then the parameters are ignored.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
    {
        Write(forecolor, backcolor, message, args);
        WriteLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if there are no more lines available.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => Console.ReadLine();

    /// <summary>
    /// Reads the next available line from the console, using the given foregroubd color. Returns
    /// either the read string, or null if there are no more lines available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = ReadLine();
        Console.ForegroundColor = old;        
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console, using the given foreground and background
    /// colors. Returns either the read string, or null if there are no more lines available.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldfg = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldbk = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = ReadLine();        
        Console.ForegroundColor = oldfg;
        Console.BackgroundColor = oldbk;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console waiting for at most the given amount of
    /// time. Returns either the read string, or null if no more lines were available, or if the
    /// line edition was cancelled.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static string? ReadLine(TimeSpan timeout)
    {
        var r = EditLine(string.Empty, timeout, out var value);
        return r ? value : null;
    }

    /// <summary>
    /// Reads the next available line from the console waiting for at most the given amount of
    /// time, using the given foreground color. Returns either the read string, or null if no
    /// more lines were available, or if the line edition was cancelled.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor, TimeSpan timeout)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = ReadLine(timeout);
        Console.ForegroundColor = old;
        return r;
    }

    /// <summary>
    /// Reads the next available line from the console waiting for at most the given amount of
    /// time, using the given foreground and background colors. Returns either the read string,
    /// or null if no more lines were available, or if the line edition was cancelled.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout)
    {
        var oldfg = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldbk = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = ReadLine(timeout);
        Console.ForegroundColor = oldfg;
        Console.BackgroundColor = oldbk;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, using the given foreground color. Returns
    /// <c>true</c> if the edition was completed, or <c>false</c> otherwise. In the former case,
    /// the out argument carries the editted string.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, string source, [NotNullWhen(true)] out string? value)
        => EditLine(forecolor, source, Timeout.InfiniteTimeSpan, out value);

    /// <summary>
    /// Edits in the console the given source string, waiting for at most the given amount of
    /// time to complete the edition, using the given foreground color. Returns <c>true</c> if
    /// such has happened, or <c>false</c> if the timeout expired or if the edition was cancelled.
    /// In the former case, the out argument carries the editted string.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, string source, TimeSpan timeout, [NotNullWhen(true)] out string? value)
    {
        var oldfg = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var r = EditLine(source, timeout, out value);
        Console.ForegroundColor = oldfg;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given foreground and background
    /// colors. Returns <c>true</c> if the edition was completed, or <c>false</c> otherwise. In
    /// the former case, the out argument carries the editted string.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string source, [NotNullWhen(true)] out string? value)
        => EditLine(forecolor, backcolor, source, Timeout.InfiniteTimeSpan, out value);

    /// <summary>
    /// Edits in the console the given source string, waiting for at most the given amount of
    /// time to complete the edition, using the given foreground and background colors. Returns
    /// <c>true</c> if such has happened, or <c>false</c> if the timeout expired or if the edition
    /// was cancelled. In the former case, the out argument carries the editted string.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string source, TimeSpan timeout, [NotNullWhen(true)] out string? value)
    {
        var oldfg = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldbk = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var r = EditLine(source, timeout, out value);
        Console.ForegroundColor = oldfg;
        Console.BackgroundColor = oldbk;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string. Returns <c>true</c> if the edition was
    /// completed, or <c>false</c> otherwise. In the former case, the out argument carries the
    /// editted string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        string source, [NotNullWhen(true)] out string? value)
        => EditLine(source, Timeout.InfiniteTimeSpan, out value);

    /// <summary>
    /// Edits in the console the given source string, waiting for at most the given amount of
    /// time to complete the edition. Returns <c>true</c> if such has happened, or <c>false</c>
    /// if the timeout expired or if the edition was cancelled. In the former case, the out
    /// argument carries the editted string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static bool EditLine(string source, TimeSpan timeout, [NotNullWhen(true)] out string? value)
    {
        var sb = new StringBuilder(source ?? string.Empty);
        var pos = sb.Length;
        var left = Console.CursorLeft;
        var size = Console.CursorSize;
        var insert = false;
        int len;

        // Main loop...
        while (true)
        {
            var info = ReadKey(timeout, false);
            info ??= new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            // Special keys...
            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    SetInsert(false);
                    Console.WriteLine();
                    value = sb.ToString();
                    return true;

                case ConsoleKey.Escape:
                    SetInsert(false);
                    len = sb.Length; sb.Clear();
                    ShowLine(0, len);
                    Console.WriteLine();
                    value = null;
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

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next key pressed in the console. The pressed key is displayed in the console
    /// if requested (by default).
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool display = true) => Console.ReadKey(!display);

    /// <summary>
    /// Reads the next key pressed in the console, waiting for at most the given amount of time.
    /// Returns either the pressed key, or <c>null</c> if the timeout period has expired. The
    /// pressed key is displayed in the console if requested (by default).
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool display = true)
    {
        var ms = timeout.ValidateTimeout();
        var ini = DateTime.UtcNow;

        while (true)
        {
            if (Console.KeyAvailable) return Console.ReadKey(!display);

            if (ms != -1)
            {
                var now = DateTime.UtcNow;
                if ((now - ini) > timeout) return null;
            }
            Thread.Sleep(0);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Clears the console.
    /// </summary>
    public static void Clear() => Console.Clear();
}