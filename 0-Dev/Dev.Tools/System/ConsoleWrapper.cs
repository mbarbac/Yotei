using Microsoft.Win32.SafeHandles;

namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a wrapper over the <see cref="_Console"/> class.
/// </summary>
public static class ConsoleWrapper
{
    /// <summary>
    /// Determines if the console output shall also be emitted into debug, or not.
    /// </summary>
    public static bool UseDebug { get; set; } = false;
    static bool ForDebug => Ambient.IsDebug && UseDebug;

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
    /// Writes a formatted message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        if (ForDebug)
        {
            _Debug.Write(message);
            if (!Ambient.IsDebugOnConsole()) _Console.Write(message);
        }
        else _Console.Write(message);
    }

    /// <summary>
    /// Writes a formatted message, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        Write(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Writes a new line.
    /// </summary>
    public static void WriteLine()
    {
        if (ForDebug)
        {
            _Debug.WriteLine(string.Empty);
            if (!Ambient.IsDebugOnConsole()) _Console.WriteLine(string.Empty);
        }
        else _Console.WriteLine(string.Empty);
    }

    /// <summary>
    /// Writes a formatted message, followed by a new line.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(string message, params object?[] args)
    {
        message = message.FormatMessage(args);

        if (ForDebug)
        {
            _Debug.WriteLine(message);
            if (!Ambient.IsDebugOnConsole()) _Console.WriteLine(message);
        }
        else _Console.WriteLine(message);
    }

    /// <summary>
    /// Writes a formatted message, using the given color, followed by a new line.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(Color color, string message, params object?[] args)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        WriteLine(message, args);
        _Console.ForegroundColor = old;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reads the next line of characters from the console. Returns an empty string if there
    /// are no more lines available.
    /// </summary>
    /// <returns></returns>
    public static string ReadLine()
    {
        var str = _Console.ReadLine();
        return str ?? string.Empty;
    }

    /// <summary>
    /// Reads the next line of characters from the console, using the given color. Returns an
    /// empty string if there are no more lines available.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ReadLine(Color color)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        var str = ReadLine(); _Console.ForegroundColor = old;
        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the line of characters editted in the console.
    /// </summary>
    /// <returns></returns>
    public static string EditLine() => EditLine(string.Empty, out _);

    /// <summary>
    /// Returns the line of characters editted in the console, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string EditLine(Color color) => EditLine(color, string.Empty, out _);

    /// <summary>
    /// Returns the given value editted in the console.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string EditLine(string value) => EditLine(value, out _);

    /// <summary>
    /// Returns the given value editted in the console, using the given color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string EditLine(Color color, string value) => EditLine(color, value, out _);

    /// <summary>
    /// Returns the given value editted in the console, using the given color. The out
    /// <paramref name="info"/> argument contains the console key that finished the editting.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="value"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EditLine(Color color, string value, out ConsoleKeyInfo info)
    {
        var old = _Console.ForegroundColor; _Console.ForegroundColor = color;
        var str = EditLine(value, out info); _Console.ForegroundColor = old;
        return str;
    }

    /// <summary>
    /// Returns the given value editted in the console. The out <paramref name="info"/> argument
    /// contains the console key that finished the editting.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string EditLine(string value, out ConsoleKeyInfo info)
    {
        value = value.ThrowIfNull();

        var sb = new StringBuilder(value);
        var pos = sb.Length;
        var ini = _Console.CursorLeft;
        var insert = false;
        var size = _Console.CursorSize;
        var temp = 0;

        ShowLine(ini + pos);
        while (true)
        {
            info = _Console.ReadKey(true);

            if (IsValid(info.KeyChar))
            {
                ClearLine();
                if (insert) sb.Insert(pos, info.KeyChar);
                else
                {
                    if (pos < sb.Length) sb[pos] = info.KeyChar;
                    else sb.Append(info.KeyChar);
                }
                pos++;
                ShowLine(ini + pos);
            }

            switch (info.Key)
            {
                case ConsoleKey.Enter:
                    ShowLine(ini + sb.Length);
                    if (OperatingSystem.IsWindows()) _Console.CursorSize = size;
                    WriteLine();
                    return sb.ToString();

                case ConsoleKey.Escape:
                    sb = new StringBuilder(value);
                    ShowLine(ini + sb.Length);
                    if (OperatingSystem.IsWindows()) _Console.CursorSize = size;
                    WriteLine();
                    return value;

                case ConsoleKey.Insert:
                    insert = !insert;
                    temp = insert ? 100 : size;
                    if (OperatingSystem.IsWindows()) _Console.CursorSize = temp;
                    break;

                case ConsoleKey.Home:
                    pos = 0; _Console.CursorLeft = ini;
                    break;

                case ConsoleKey.End:
                    pos = sb.Length; _Console.CursorLeft = ini + sb.Length;
                    break;

                case ConsoleKey.LeftArrow:
                    while (pos > 0)
                    {
                        pos--; _Console.CursorLeft--;

                        if (pos > 0 &&
                            info.Modifiers.HasFlag(ConsoleModifiers.Control) &&
                            char.IsLetterOrDigit(sb[pos - 1]))
                            continue;

                        break;
                    }
                    break;

                case ConsoleKey.RightArrow:
                    while (pos < sb.Length)
                    {
                        pos++; _Console.CursorLeft++;

                        if (pos < sb.Length &&
                            info.Modifiers.HasFlag(ConsoleModifiers.Control) &&
                            char.IsLetterOrDigit(sb[pos - 1]))
                            continue;

                        break;
                    }
                    break;

                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        ClearLine(); while (pos > 0)
                        {
                            pos--; sb.Remove(pos, 1);
                            if (!info.Modifiers.HasFlag(ConsoleModifiers.Control)) break;
                        }
                        ShowLine(ini + pos);
                    }
                    break;

                case ConsoleKey.Delete:
                    if (pos < sb.Length)
                    {
                        ClearLine(); while (pos < sb.Length)
                        {
                            sb.Remove(pos, 1);
                            if (!info.Modifiers.HasFlag(ConsoleModifiers.Control)) break;
                        }
                        ShowLine(ini + pos);
                    }
                    break;
            }
        }

        // Clears the editting line, and sets the cursor at the ini position...
        void ClearLine()
        {
            var header = new string(' ', sb!.Length);
            _Console.CursorLeft = ini;
            _Console.Write(header);
            _Console.CursorLeft = ini;
        }

        // Shows again the editting line, setting the cursor at the given position...
        void ShowLine(int cursor)
        {
            _Console.CursorLeft = ini;
            _Console.Write(sb);
            _Console.CursorLeft = cursor;
        }

        // Determines if the given character is a valid one or not...
        bool IsValid(char c)
        {
            if (char.IsLetterOrDigit(c)) return true;
            if (Array.IndexOf(ValidChars, c) >= 0) return true;            
            return false;
        }
    }
    static char[] ValidChars = " .,;:_\\+-*/{}[]()!@#$%&¿?`´'\"".ToCharArray();

    // ----------------------------------------------------

    /// <inheritdoc cref="_Console.ReadKey(bool)">
    /// </inheritdoc>
    public static ConsoleKeyInfo ReadKey(bool intercept = false) => _Console.ReadKey(intercept);

    /// <summary>
    /// Obtains the next character or function key pressed by the user, optionally presenting
    /// it in the console, or null if the wait time period has exhausted. A wait period of
    /// zero means waiting forever.
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan wait, bool intercept = false)
    {
        var ini = DateTime.UtcNow;
        while (true)
        {
            if (_Console.KeyAvailable) return _Console.ReadKey(intercept);

            if (wait != TimeSpan.Zero)
            {
                var now = DateTime.UtcNow;
                if (now - ini > wait) return null;
                Thread.Sleep(0);
            }
        }
    }
}