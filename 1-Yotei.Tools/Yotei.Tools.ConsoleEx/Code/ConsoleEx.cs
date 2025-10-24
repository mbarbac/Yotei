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
        /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(bool debug, string message, params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Console.Write(message);
            if (debug) WithNoConsoleListeners(() => Debug.Write(message));
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(
            bool debug,
            ConsoleColor forecolor,
            string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
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
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(debug, forecolor, message, args);
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
        /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(bool debug, string message, params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Console.WriteLine(message);
            if (debug) WithNoConsoleListeners(() => Debug.WriteLine(message));
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
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
            Console.WriteLine(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
        /// If '<paramref name="debug"/>' is requested, the message is also written in the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor,
            string message, params object?[]? args)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(debug, forecolor, message, args);
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
        /// <inheritdoc cref="Console.ReadLine"/>
        /// If '<paramref name="debug"/>' is requested, the result is also written in the debug
        /// environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string? ReadLine(bool debug)
        {
            var str = Console.ReadLine();
            if (debug && str is not null) WithNoConsoleListeners(() => Debug.WriteLine(str));
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// If '<paramref name="debug"/>' is requested, the result is also written in the debug
        /// environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLine(bool debug, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = Console.ReadLine(debug);
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// If '<paramref name="debug"/>' is requested, the result is also written in the debug
        /// environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = Console.ReadLine(debug, forecolor);
            Console.BackgroundColor = oldback;
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo ReadKey(bool debug, bool intercept)
        {
            var info = Console.ReadKey(intercept);
            if (debug) WithNoConsoleListeners(() =>
            {
                var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                Debug.Write(ch);
            });
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo ReadKey(
            bool debug,
            ConsoleColor forecolor, bool intercept)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKey(debug, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo ReadKey(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor, bool intercept)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKey(debug, forecolor, intercept);
            Console.BackgroundColor = oldback;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any. Returns '<c>null</c> if no key was pressed
        /// when the timeout expired.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(bool debug, TimeSpan timeout, bool intercept)
        {
            var ms = timeout.ValidatedTimeout;
            var ini = DateTime.UtcNow;

            while (true)
            {
                // Trying an available key...
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(intercept: true);
                    var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";

                    if (!intercept) Console.Write(ch);
                    if (debug) WithNoConsoleListeners(() => Debug.Write(ch));
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
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any. Returns '<c>null</c> if no key was pressed
        /// when the timeout expired.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            bool debug, ConsoleColor forecolor, TimeSpan timeout, bool intercept)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = Console.ReadKey(debug, timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. If '<paramref name="debug"/>' is requested, the result is also
        /// written in the debug environment, if any. Returns '<c>null</c> if no key was pressed
        /// when the timeout expired.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, bool intercept)
        {
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = Console.ReadKey(debug, forecolor, timeout, intercept);
            Console.BackgroundColor = oldback;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. Returns '<c>null</c> if no key was pressed when the timeout
        /// expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool intercept) => ReadKey(false, timeout, intercept);

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. Returns '<c>null</c> if no key was pressed when the timeout
        /// expired.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            ConsoleColor forecolor, TimeSpan timeout, bool intercept)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = Console.ReadKey(timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/> Use '<c>false</c> to prevent interception
        /// and display the key. Returns '<c>null</c> if no key was pressed when the timeout
        /// expired.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, bool intercept)
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
        /// expired. Null source strings are treated as empty ones. If '<paramref name="debug"/>'
        /// is requested, the result is also written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(bool debug, TimeSpan timeout, string? source = null)
            => EditLine(debug, Console.ForegroundColor, Console.BackgroundColor, timeout, source);

        /// <summary>
        /// Edits in the console the given source string and returns the result of that edition,
        /// or <c>null</c> if the user cancelled it by pressing [Escape], or if the timeout has
        /// expired. Null source strings are treated as empty ones. If '<paramref name="debug"/>'
        /// is requested, the result is also written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            bool debug,
            ConsoleColor forecolor, TimeSpan timeout, string? source = null)
            => EditLine(debug, forecolor, Console.BackgroundColor, timeout, source);

        /// <summary>
        /// Edits in the console the given source string and returns the result of that edition,
        /// or <c>null</c> if the user cancelled it by pressing [Escape], or if the timeout has
        /// expired. Null source strings are treated as empty ones. If '<paramref name="debug"/>'
        /// is requested, the result is also written in the debug environment, if any.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, string? source = null)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var size = Console.CursorSize;
            var left = Console.CursorLeft;
            bool insert;

            var sb = new StringBuilder(); sb.Append(source ?? string.Empty);
            var pos = sb.Length;
            int len;

            SetInsertSize(insert = true);
            ShowLine(pos);

            while (true)
            {
                var info = ReadKey(timeout, intercept: true);
                info ??= new('\0', ConsoleKey.Escape, false, false, false);

                /// <summary>
                /// Special keys.
                /// </summary>
                switch (info.Value.Key)
                {
                    case ConsoleKey.Enter:
                        SetInsertSize(true);
                        Console.WriteLine();
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        return sb.ToString();

                    case ConsoleKey.Escape:
                        SetInsertSize(true);
                        len = sb.Length; sb.Clear(); ShowLine(0, len);
                        Console.WriteLine();
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        return null;

                    case ConsoleKey.Insert:
                        SetInsertSize(insert = !insert);
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

                /// <summary>
                /// Special keys.
                /// </summary>
                if (info.Value.KeyChar >= ' ')
                {
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
            }

            /// <summary>
            /// Sets the cursor size for the insert mode ON (true) or OFF (false).
            /// </summary>
            void SetInsertSize(bool value)
            {
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (windows) Console.CursorSize = value ? size : 100;
            }

            /// <summary>
            /// Shows the current value clearing up to the remaining len, and sets the cursor
            /// position to the given one.
            /// </summary>
            void ShowLine(int pos, int len = 0)
            {
                Console.CursorLeft = left;
                Console.Write(sb);

                Console.ForegroundColor = oldfore;
                Console.BackgroundColor = oldback;
                len -= sb.Length;
                if (len > 0) Console.Write(Header(len));
                Console.CursorLeft = left + pos;

                Console.ForegroundColor = forecolor;
                Console.BackgroundColor = backcolor;
            }
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
        public static string? EditLine(
            TimeSpan timeout, string? source = null) => EditLine(false, timeout, source);

        /// <summary>
        /// Edits in the console the given source string and returns the result of that edition,
        /// or <c>null</c> if the user cancelled it by pressing [Escape], or if the timeout has
        /// expired. Null source strings are treated as empty ones.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            ConsoleColor forecolor, TimeSpan timeout, string? source = null)
            => EditLine(false, forecolor, timeout, source);

        /// <summary>
        /// Edits in the console the given source string and returns the result of that edition,
        /// or <c>null</c> if the user cancelled it by pressing [Escape], or if the timeout has
        /// expired. Null source strings are treated as empty ones.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            ConsoleColor forecolor, ConsoleColor backcolor, TimeSpan timeout, string? source = null)
            => EditLine(false, forecolor, backcolor, timeout, source);
    }

    // ====================================================

    /// <summary>
    /// Executes the given action with no active console listerners.
    /// </summary>
    static void WithNoConsoleListeners(Action action)
    {
        lock (Ambient.Lock)
        {
            var items = Ambient.GetConsoleListeners().ToArray();
            Ambient.RemoveListeners(items);

            try { action(); }
            finally { Ambient.AddListeners(items); }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string header with the requested number of spaces.
    /// </summary>
    static string Header(int size)
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