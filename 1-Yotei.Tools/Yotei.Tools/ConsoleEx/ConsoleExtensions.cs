using Color = System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(Color forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(
            Color forecolor, Color backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(message, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(bool debug, string message, params object?[]? args)
        {
            var str = FormatMessage(message, args);
            Console.Write(str);
            DebugWrite(debug, str);
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug, Color forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = FormatMessage(message, args);
            Console.Write(str);
            DebugWrite(debug, str);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug, Color forecolor, Color backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = FormatMessage(message, args);
            Console.Write(str);
            DebugWrite(debug, str);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(Color forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            Color forecolor, Color backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(message, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        public static void WriteLineEx(bool debug)
        {
            Console.WriteLine();
            DebugWriteLine(debug, string.Empty);
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(bool debug, string message, params object?[]? args)
        {
            var str = FormatMessage(message, args);
            Console.WriteLine(str);
            DebugWriteLine(debug, str);
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug, Color forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = FormatMessage(message, args);
            Console.WriteLine(str);
            DebugWriteLine(debug, str);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug, Color forecolor, Color backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = FormatMessage(message, args);
            Console.WriteLine(str);
            DebugWriteLine(debug, str);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Returns <see langword="null"/> if any was available.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLine(Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = Console.ReadLine();
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Returns <see langword="null"/> if any was available.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLine(Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = Console.ReadLine();
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Returns <see langword="null"/> if any was available.
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug)
        {
            var str = Console.ReadLine();
            if (str != null) DebugWriteLine(debug, str);
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Returns <see langword="null"/> if any was available.
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = Console.ReadLine();
            if (str != null) DebugWriteLine(debug, str);
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/> 
        /// Returns <see langword="null"/> if any was available.
        /// If requested, that string representation is also written to the not-console-alike debug
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = Console.ReadLine();
            if (str != null) DebugWriteLine(debug, str);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout) => ReadKeyEx(false, timeout, false);

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(false, timeout, false);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(false, timeout, false);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool intercept) => ReadKeyEx(false, timeout, intercept);

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool intercept, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(false, timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool intercept, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(false, timeout, intercept);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout) => ReadKeyEx(debug, timeout, false);

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(debug, timeout, false);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(debug, timeout, false);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout, bool intercept)
        {
            var ini = DateTime.UtcNow;
            var ms = timeout.ValidatedTimeout;

            while (true)
            {
                // Trying an available key...
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(intercept);
                    DebugWriteLine(debug, ToHumanString(info));
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
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, bool intercept, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(debug, timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadKey(bool)"/>
        /// Returns <see langword="null"/> if the timeout period expired.
        /// If requested, a string representation of the character or function key is also written
        /// to the not-console-alike debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, bool intercept, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(debug, timeout, intercept);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source = null)
            => EditLineEx(false, Timeout.InfiniteTimeSpan, Console.ForegroundColor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            Color forecolor, string? source = null)
            => EditLineEx(false, Timeout.InfiniteTimeSpan, forecolor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            Color forecolor, Color backcolor, string? source = null)
            => EditLineEx(false, Timeout.InfiniteTimeSpan, forecolor, backcolor, source);

        // ------------------------------------------------

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            TimeSpan timeout, string? source = null)
            => EditLineEx(false, timeout, Console.ForegroundColor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            TimeSpan timeout, Color forecolor, string? source = null)
            => EditLineEx(false, timeout, forecolor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            TimeSpan timeout, Color forecolor, Color backcolor, string? source = null)
            => EditLineEx(false, timeout, forecolor, backcolor, source);

        // ------------------------------------------------

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source = null)
            => EditLineEx(debug, Timeout.InfiniteTimeSpan, Console.ForegroundColor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, Color forecolor, string? source = null)
            => EditLineEx(debug, Timeout.InfiniteTimeSpan, forecolor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, Color forecolor, Color backcolor, string? source = null)
            => EditLineEx(debug, Timeout.InfiniteTimeSpan, forecolor, backcolor, source);

        // ------------------------------------------------

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, TimeSpan timeout, string? source = null)
            => EditLineEx(debug, timeout, Console.ForegroundColor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, TimeSpan timeout, Color forecolor, string? source = null)
            => EditLineEx(debug, timeout, forecolor, Console.BackgroundColor, source);

        /// <summary>
        /// Returns either the result of editting the given source string in the console, or null
        /// if such edition was cancelled or the timeout period has expired.
        /// If requested, the resulting string, if any, is also written to the not-console-alike
        /// debug listeners.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, TimeSpan timeout, Color forecolor, Color backcolor, string? source = null)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var oldsize = Console.CursorSize;
            int oldleft = Console.CursorLeft;

            var sb = new StringBuilder(source ?? string.Empty);
            var pos = sb.Length;
            bool insert;
            int len;

            SetInsertMode(false);
            ShowLine(pos);

            while (true)
            {
                // Capturing, or ESC if timeout...
                var info = ReadKey(timeout, intercept: true);
                info ??= new('\0', ConsoleKey.Escape, false, false, false);

                // Special keys...
                switch (info.Value.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        SetInsertMode(false);
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        DebugWriteLine(debug, sb.ToString());
                        return sb.ToString();

                    case ConsoleKey.Escape:
                        len = sb.Length; sb.Clear(); ShowLine(0, len);
                        Console.WriteLine();
                        SetInsertMode(false);
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        return null;

                    case ConsoleKey.Insert:
                        SetInsertMode(!insert);
                        break;

                    case ConsoleKey.Home:
                        pos = 0; Console.CursorLeft = oldleft;
                        break;

                    case ConsoleKey.End:
                        pos = sb.Length; Console.CursorLeft = oldleft + pos;
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
                        if (pos > 0)
                        {
                            if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                            {
                                do
                                {
                                    pos--; Console.CursorLeft--;
                                    if (pos == 0) break;
                                    if (char.IsLetterOrDigit(sb[pos]) &&
                                        !char.IsLetterOrDigit(sb[pos - 1])) break;
                                }
                                while (true);
                            }
                            else { pos--; Console.CursorLeft--; }
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (pos < sb.Length)
                        {
                            if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                            {
                                do
                                {
                                    pos++; Console.CursorLeft++;
                                    if (pos == sb.Length) break;
                                    if (char.IsLetterOrDigit(sb[pos]) &&
                                        !char.IsLetterOrDigit(sb[pos - 1])) break;
                                }
                                while (true);
                            }
                            else { pos++; Console.CursorLeft++; }
                        }
                        break;
                }

                // Standard keys...
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
                        continue;
                    }
                    else
                    {
                        sb.Append(info.Value.KeyChar);
                        Console.Write(info.Value.KeyChar);
                        pos++;
                        continue;
                    }
                }
            }

            /// <summary>
            /// Shows the current value, clearing up to the remaining len.
            /// Sets the cursor position to the given one.
            /// </summary>
            void ShowLine(int pos, int len = 0)
            {
                Console.CursorLeft = oldleft;
                Console.Write(sb.ToString());

                len -= sb.Length; if (len > 0)
                {
                    Console.ForegroundColor = oldfore;
                    Console.BackgroundColor = oldback;
                    Console.Write(FromSpaces(len));
                    Console.ForegroundColor = forecolor;
                    Console.BackgroundColor = backcolor;
                }
                Console.CursorLeft = oldleft + pos;
            }

            /// <summary>
            /// Sets the insert mode to the given value.
            /// The setter is only supported in windows.
            /// </summary>
            void SetInsertMode(bool value)
            {
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (windows) Console.CursorSize = value ? 100 : oldsize;
                insert = value;
            }
        }
    }
}