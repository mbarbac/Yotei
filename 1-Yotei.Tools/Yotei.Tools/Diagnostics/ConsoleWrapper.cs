using Color = System.ConsoleColor;

namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Represents a wrapper over the <see cref="Console"/> class.
/// </summary>
public static class ConsoleWrapper
{
    private const bool DEFAULTDEBUG = false; // Do not change value!

    // ----------------------------------------------------

    /// <summary>
    /// Writes in the console an empty message.
    /// </summary>
    public static void Write() { }

    /// <summary>
    /// Writes in the console an empty message, and replicates it in the debug listeners, if
    /// requested.
    /// </summary>
    /// <param name="debug"></param>
    [SuppressMessage("", "IDE0060")]
    public static void Write(bool debug) { }

    /// <summary>
    /// Writes in the console a formatted message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string? message, params object?[] args)
    {
        Write(DEFAULTDEBUG, message, args);
    }

    /// <summary>
    /// Writes in the console a formatted message, and replicates it in the debug listeners if
    /// such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= DiagnosticsWrapper.NullArray;
        if (args.Length > 0) message = string.Format(message, args);

        if (debug)
        {
            Debug.Write(message);

            if (message.EndsWith("\n") ||
                message.EndsWith(Environment.NewLine))
                DiagnosticsWrapper.DebugAtOrigin = true;

            if (Ambient.IsConsoleListener()) return;
        }

        Console.Write(message);
    }

    /// <summary>
    /// Writes in the console a formatted message, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(Color color, string? message, params object?[] args)
    {
        Write(DEFAULTDEBUG, color, message, args);
    }

    /// <summary>
    /// Writes in the console a formatted message, using the given color, and replicates it in
    /// the debug listeners if requested.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, Color color, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        Write(debug, message, args);
        Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes in the console an empty message followed by a new line.
    /// </summary>
    public static void WriteLine() => WriteLine(DEFAULTDEBUG);

    /// <summary>
    /// Writes in the console an empty message followed by a new line, and replicates it in the
    /// debug listeners if requested.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

    /// <summary>
    /// Writes in the console a formatted message followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string? message, params object?[] args)
    {
        WriteLine(DEFAULTDEBUG, message, args);
    }

    /// <summary>
    /// Writes in the console a formatted message followed by a new line, and replicates it in
    /// the debug listeners if requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, string? message, params object?[] args)
    {
        message ??= string.Empty;
        args ??= DiagnosticsWrapper.NullArray;
        if (args.Length > 0) message = string.Format(message, args);

        if (debug)
        {
            Debug.WriteLine(message);

            if (message.EndsWith("\n") ||
                message.EndsWith(Environment.NewLine))
                DiagnosticsWrapper.DebugAtOrigin = true;

            if (Ambient.IsConsoleListener()) return;
        }

        Console.WriteLine(message);
    }

    /// <summary>
    /// Writes in the console a formatted message followed by a new line, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(Color color, string? message, params object?[] args)
    {
        WriteLine(DEFAULTDEBUG, color, message, args);
    }

    /// <summary>
    /// Writes in the console a formatted message followed by a new line, using the given color,
    /// and replicates it in the debug listeners if requested.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, Color color, string? message, params object?[] args)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        WriteLine(debug, message, args);
        Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next available line from the console.
    /// Returns null if no more lines were available, or the first available line otherwise.
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => Console.ReadLine();

    /// <summary>
    /// Reads the next available line from the console, using the given color. Returns null if
    /// no more lines were available, or the first available line otherwise.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string? ReadLine(Color color)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        var str = ReadLine();
        Console.ForegroundColor = old;
        return str;
    }

    /// <summary>
    /// Reads the next available line from the console, waiting for at most the given amount of
    /// time. Returns null if no more lines were available, or the first available line otherwise.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static string? ReadLine(TimeSpan timeout)
    {
        EditLine(string.Empty, timeout, out var value);
        return value;
    }

    /// <summary>
    /// Reads the next available line from the console, using the given color, and waiting for at
    /// most the given amount of time. Returns null if no more lines were available, or the first
    /// available line otherwise.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static string? ReadLine(Color color, TimeSpan timeout)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        var str = ReadLine(timeout);
        Console.ForegroundColor = old;
        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, using the given color. Returns true if the
    /// user finished the edit pressing the [Enter] key, or false if the user finished it with
    /// the [Escape] key.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(string source, [NotNullWhen(true)] out string? value)
    {
        return EditLine(source, Timeout.InfiniteTimeSpan, out value);
    }

    /// <summary>
    /// Edits in the console the given source string. Returns true if the user finished the edit
    /// pressing the [Enter] key, or false if the user finished it with the [Escape] key.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="color"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(string source, Color color, [NotNullWhen(true)] out string? value)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        var r = EditLine(source, out value);
        Console.ForegroundColor = old;
        return r;
    }

    /// <summary>
    /// Edits in the console the given source string waiting for at most the given amount of time.
    /// Returns true if the user finished the edit pressing the [Enter] key, or false if the user
    /// finished it with the [Escape] key or if the timeout expired.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(
        string source, TimeSpan timeout, [NotNullWhen(true)] out string? value)
    {
        var sb = new StringBuilder(source ?? string.Empty);
        var pos = sb.Length;
        var left = Console.CursorLeft;
        var size = Console.CursorSize;
        var insert = false;
        int len;

        SetInsert(false);
        ShowLine(pos);
        while (true)
        {
            // Timeout...
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

            // Regular chars...
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

        // Shows the current value, sets the cursor position, and clears the remainign len.
        void ShowLine(int pos, int len = 0)
        {
            Console.CursorLeft = left;
            Console.Write(sb);
            len -= sb.Length;
            if (len > 0) Console.Write(DiagnosticsWrapper.Header(len));
            Console.CursorLeft = left + pos;
        }

        /// Sets the cursor to insert mode, or not.
        void SetInsert(bool value)
        {
            if (value == insert) return;

            var temp = value ? 25 : size;
            var iswindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (iswindows) Console.CursorSize = temp;
            insert = value;
        }
    }

    /// <summary>
    /// Edits in the console the given source string waiting for at most the given amount of time,
    /// and using the given color. Returns true if the user finished the edit pressing the [Enter]
    /// key, or false if the user finished it with the [Escape] key or if the timeout expired.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EditLine(string source, Color color, TimeSpan timeout, out string? value)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        var r = EditLine(source, timeout, out value);
        Console.ForegroundColor = old;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next character of function key pressed by the user. The key pressed by the user
    /// is displayed in the console window if requested.
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool display = true) => Console.ReadKey(!display);

    /// <summary>
    /// Reads the next character of function key pressed by the user, waiting for at most the
    /// given amount of time. Returns either the pressed key, or <c>null</c> if the timeout
    /// period has expired. The key pressed by the user is displayed in the console window if
    /// requested.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool display = true)
    {
        var ms = ValidateTimeout(timeout);
        var ini = DateTime.UtcNow;

        while (true)
        {
            if (Console.KeyAvailable) return Console.ReadKey(!display);

            if (ms != -1)
            {
                var now = DateTime.UtcNow;
                if ((now - ini) > timeout) return null;
            }
            Thread.Sleep(1);
        }

        // Validates the given timeout...
        static long ValidateTimeout(TimeSpan timeout)
        {
            var ms = (long)timeout.TotalMilliseconds;
            if (ms is < (-1) or > uint.MaxValue) throw new ArgumentOutOfRangeException(
                nameof(timeout),
                $"Invalid timeout: {timeout}");

            return ms;
        }
    }
}