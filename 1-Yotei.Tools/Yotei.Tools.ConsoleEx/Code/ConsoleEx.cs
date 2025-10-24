namespace Yotei.Tools;

// ========================================================
public static class ConsoleEx
{
    extension(Console)
    {
        /// <summary>
        /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(forecolor, message, args);
            Console.BackgroundColor = oldback;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(forecolor, message, args);
            Console.BackgroundColor = oldback;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLine(ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = Console.ReadLine();
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = Console.ReadLine(forecolor);
            Console.BackgroundColor = oldback;
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> But only if a key was obtained and if the
        /// <paramref name="intercept"/> value is '<c>false</c>' to prevent interception. Returns
        /// '<c>null</c>' if no key was pressed and the timeout expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool intercept = false)
        {
            var ms = timeout.ValidatedTimeout;
            var ini = DateTime.UtcNow;

            while (true)
            {
                // Trying an available key...
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(intercept: true);
                    if (!intercept)
                    {
                        var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                        Console.Write(ch);
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
                Thread.Yield();
            }

            throw null;
        }

        /// <summary>
        /// <inheritdoc cref="ReadKey(TimeSpan, bool)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            ConsoleColor forecolor, TimeSpan timeout, bool intercept = false)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = Console.ReadKey(timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="ReadKey(TimeSpan, bool)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, bool intercept = false)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = Console.ReadKey(forecolor, timeout, intercept);
            Console.BackgroundColor = oldback;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string and returns the result of that edition,
        /// or <c>null</c> if the user cancelled it by pressing [Escape], or if the timeout has
        /// expired. Null source strings are treated as empty ones.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(TimeSpan timeout, string? source = null)
        {
            var size = Console.CursorSize;
            var left = Console.CursorLeft;
            bool insert;

            var sb = new StringBuilder(); sb.Append(source ?? string.Empty);
            var pos = sb.Length;
            int len;

            SetInsert(insert = false);
            ShowLine(pos);

            while (true)
            {
                var info = ReadKey(timeout, intercept: true);
                info ??= new('\0', ConsoleKey.Escape, false, false, false);

                // Special keys...
                switch (info.Value.Key)
                {
                    case ConsoleKey.Enter:
                        SetInsert(false);
                        Console.WriteLine();
                        return sb.ToString();

                    case ConsoleKey.Escape:
                        SetInsert(false);
                        len = sb.Length; sb.Clear(); ShowLine(0, len);
                        Console.WriteLine();
                        return null;

                    case ConsoleKey.Insert:
                        SetInsert(insert = !insert);
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
            /// Sets the insert mode to ON (true) or OFF (false).
            /// </summary>
            void SetInsert(bool value)
            {
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (windows) Console.CursorSize = value ? 100 : size;
            }

            /// <summary>
            /// Shows the current value, clears up to remaining len, and sets cursor's position.
            /// </summary>
            void ShowLine(int pos, int len = 0)
            {
                Console.CursorLeft = left;
                Console.Write(sb);

                len -= sb.Length;
                if (len > 0) Console.Write(Header(len));
                Console.CursorLeft = left + pos;
            }
        }

        /// <summary>
        /// <inheritdoc cref="EditLine(TimeSpan, string?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            ConsoleColor forecolor, TimeSpan timeout, string? source = null)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var value = Console.EditLine(timeout, source);
            Console.ForegroundColor = oldfore;
            return value;
        }

        /// <summary>
        /// <inheritdoc cref="EditLine(TimeSpan, string?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, string? source = null)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var value = Console.EditLine(forecolor, timeout, source);
            Console.BackgroundColor = oldback;
            return value;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string header with the requested number of spaces.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string Header(int size)
    {
        if (size < 0) throw new ArgumentException("Size cannot be negative.").WithData(size);

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

        if (!Headers.TryGetValue(size, out var header))
        {
            Headers.Add(size, header = new(Space, size));
        }
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