#pragma warning disable IDE0075

using Yotei.Tools.Diagnostics;

namespace Yotei.Tools.ConsoleEx;

// ========================================================
/// <summary>
/// Represents an extended <see cref="Console"/> class.
/// TODO: Use C# 14 static extension methods to extend 'Console' capabilities.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Clears the console.
    /// </summary>
    public static void Clear() => Console.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        string? message, params object?[] args) => Write(false, message, args);

    /// <summary>
    /// Writes the given message.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
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
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => Write(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);
        Console.Write(message);
        if (debug) DebugEx.Write(false, message);
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        Write(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message and, if requested, replicates it to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
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
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        string? message, params object?[] args) => WriteLine(false, message, args);

    /// <summary>
    /// Writes the given message, followed by a new line.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
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
    public static void WriteLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
        => WriteLine(false, forecolor, backcolor, message, args);

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line and, if requested, to the debug environment.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, string? message, params object?[] args)
    {
        if (message is null) return;
        if (message.Length == 0) return;

        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);
        Console.WriteLine(message);
        if (debug) DebugEx.WriteLine(false, message);
    }

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor, string? message, params object?[] args)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        WriteLine(debug, message, args);
        Console.ForegroundColor = oldfore;
    }

    /// <summary>
    /// Writes the given message, followed by a new line and, if requested, replicates it to the
    /// debug environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor, string? message, params object?[] args)
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
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor) => ReadLine(false, forecolor);

    /// <summary>
    /// Reads the next available line from the console. Returns either the read string, or null
    /// if no more lines are available.
    /// </summary>
    /// <param name="debug"></param>
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
        var str = Console.ReadLine();

        if (str is not null && debug) DebugEx.WriteLine(false, str);
        return str;
    }

    /// <summary>
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
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
    /// Reads the next available line from the console, and replicates the result in the debug
    /// environment if requested. Returns either the read string, or null if no more lines are
    /// available.
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
    /// Returns the next available key from the console, or '<c>null</c>' if no key was available.
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey() => ReadKey(false, false, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Returns the next available key from the console, or '<c>null</c>' if no key was available.
    /// The key is also displayed in the console if such is requested.
    /// </summary>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool display) => ReadKey(false, display, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Reads the next available key from the console, waiting for at most the given amount of
    /// time. Returns that key or '<c>null</c>' if no key was available.The key is also displayed
    /// in the console if such is requested.
    /// </summary>
    /// <param name="display"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool display, TimeSpan timeout) => ReadKey(false, display, timeout);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next available key from the console or '<c>null</c>' if no key was available.
    /// The key is also displayed in the console and replicated in the debug environment if such
    /// is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="display"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool debug, bool display) => ReadKey(debug, display, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Reads the next available key from the console, waiting for at most the given amount of
    /// time. Returns that key or '<c>null</c>' if no key was available or if the timeout period
    /// expired. The key is also displayed in the console and replicated in the debug environment
    /// if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="display"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool debug, bool display, TimeSpan timeout)
    {
        var ms = timeout.ValidateTimeout();
        var ini = DateTime.UtcNow;

        while (true)
        {
            // There is a key to process...
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(intercept: true);
                if (display)
                {
                    var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                    if (debug) DebugEx.Write(false, ch);
                    Console.Write(ch);
                }
                return info;
            }

            // Let's wait if requested...
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
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or it was cancelled.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, Timeout.InfiniteTimeSpan, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given color, returning whether the
    /// user has accepted that edition, or it was cancelled.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var done = EditLine(source, out result);
        Console.ForegroundColor = oldfore;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given colors, returning whether
    /// the user has accepted that edition, or it was cancelled.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var done = EditLine(source, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled or the timeout expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
        => EditLine(false, timeout, source, out result);

    /// <summary>
    /// Edits in the console the given source string, using the given color, returning whether the
    /// user has accepted that edition, or either it was cancelled or the timeout expired.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var done = EditLine(timeout, source, out result);
        Console.ForegroundColor = oldfore;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given colors, returning whether
    /// the user has accepted that edition, or either it was cancelled or the timeout expired.
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var done = EditLine(timeout, source, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return done;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled. If accepted, the result is replicated in the
    /// debug environment if such is requested.
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
    /// Edits in the console the given source string, using the given color, returning whether
    /// the user has accepted that edition, or either it was cancelled. If accepted, the result
    /// is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        ConsoleColor forecolor,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var done = EditLine(debug, source, out result);
        Console.ForegroundColor = oldfore;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given colors, returning whether
    /// the user has accepted that edition, or either it was cancelled. If accepted, the result
    /// is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var done = EditLine(debug, source, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given color, returning whether
    /// the user has accepted that edition, or either it was cancelled or the timeout expired.
    /// If accepted, the result is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        ConsoleColor forecolor,
        TimeSpan timeout,
        string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var done = EditLine(debug, timeout, source, out result);
        Console.ForegroundColor = oldfore;
        return done;
    }

    /// <summary>
    /// Edits in the console the given source string, using the given colors, returning whether
    /// the user has accepted that edition, or either it was cancelled or the timeout expired. If
    /// accepted, the result is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool EditLine(
        bool debug,
        ConsoleColor forecolor, ConsoleColor backcolor,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        var done = EditLine(debug, timeout, source, out result);
        Console.ForegroundColor = oldfore;
        Console.BackgroundColor = oldback;
        return done;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string, returning whether the user has accepted
    /// that edition, or either it was cancelled or the timeout expired. If accepted, the result
    /// is replicated in the debug environment if such is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    //[SuppressMessage("", "CA1416")]
    public static bool EditLine(
        bool debug,
        TimeSpan timeout, string? source, [NotNullWhen(true)] out string? result)
    {
        var size = Console.CursorSize;
        var left = Console.CursorLeft;
        var insert = false;

        var sb = new StringBuilder(source ?? string.Empty);
        var pos = sb.Length;
        int len;

        // Main loop...
        ShowLine(pos);
        while (true)
        {
            var info = ReadKey(false, timeout);
            info ??= new('\0', ConsoleKey.Escape, false, false, false);

            // Special keys...
            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    result = sb.ToString();
                    SetInsert(false);
                    Console.WriteLine();
                    if (debug) DebugEx.WriteLine(false, result);
                    return true;

                case ConsoleKey.Escape:
                    len = sb.Length; sb.Clear();
                    result = null;
                    SetInsert(false);
                    ShowLine(0, len); Console.WriteLine();
                    return false;

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
                            var temp = char.IsLetterOrDigit(sb[pos-1]);
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

            // Regular keys...
            if (info.Value.KeyChar >= ' ')
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

        /// <summary>
        /// Sets the insert mode ON or OFF.
        /// </summary>
        void SetInsert(bool value)
        {
            var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (windows) Console.CursorSize = value ? 100 : size;
            insert = value;
        }

        /// <summary>
        /// Shows the current value, sets cursor position, and clears remanaing len...
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
}