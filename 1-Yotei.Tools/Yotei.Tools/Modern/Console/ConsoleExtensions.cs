using Color = System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// Writes to the standard output stream the given string value, formatted with the given
        /// optional arguments, if any, using the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(Color forecolor, string value, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(value, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes to the standard output stream the given string value, formatted with the given
        /// optional arguments, if any, using the given foreground and background colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        public static void Write(
            Color forecolor, Color backcolor, string value, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(value, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// Writes to the standard output stream the given string value, formatted with the given
        /// optional arguments, if any, using the given foreground color, and followed by the
        /// current line terminator.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(Color forecolor, string value, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(value, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes to the standard output stream the given string value, formatted with the given
        /// optional arguments, if any, using the given foreground and background colors, and
        /// followed by the current line terminator.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            Color forecolor, Color backcolor, string value, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(value, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, to the console
        /// and, if requested, to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(bool debug, string message, params object?[]? args)
        {
            var str = FormatMessage(message, args);
            Console.Write(str);
            TryDebugWrite(debug, str);
        }

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, to the console
        /// and, if requested, to the debug environment, using the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug, Color forecolor,
            string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, to the console
        /// and, if requested, to the debug environment, using the given foreground and background
        /// colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug, Color forecolor, Color backcolor,
            string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteEx(debug, message, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, and followed by
        /// current line terminator, to the console and, if requested, to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(bool debug, string message, params object?[]? args)
        {
            var str = FormatMessage(message, args);
            Console.WriteLine(str);
            TryDebugWriteLine(debug, str);
        }

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, and followed by
        /// current line terminator, to the console and, if requested, to the debug environment,
        /// using the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug, Color forecolor,
            string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteLineEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the given message, formatted with the given arguments, if any, and followed by
        /// current line terminator, to the console and, if requested, to the debug environment,
        /// using the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug, Color forecolor, Color backcolor,
            string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteLineEx(debug, message, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the next line of characters from the input stream, or <c>null</c> if no more
        /// characters are available, using the given foreground color.
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
        /// Returns the next line of characters from the input stream, or <c>null</c> if no more
        /// characters are available, using the given foreground and background colors.
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
        /// Returns the next line of characters from the input stream, or <c>null</c> if no more
        /// characters are available and, if requested, writes it to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug)
        {
            var str = Console.ReadLine();
            if (str != null) TryDebugWriteLine(debug, str);
            return str;
        }

        /// <summary>
        /// Returns the next line of characters from the input stream, or <c>null</c> if no more
        /// characters are available and, if requested, writes it to the debug environment, using
        /// the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, Color forecolor)
        {
            var str = Console.ReadLine(forecolor);
            if (str != null) TryDebugWriteLine(debug, str);
            return str;
        }

        /// <summary>
        /// Returns the next line of characters from the input stream, or <c>null</c> if no more
        /// characters are available and, if requested, writes it to the debug environment, using
        /// the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, Color forecolor, Color backcolor)
        {
            var str = Console.ReadLine(forecolor, backcolor);
            if (str != null) TryDebugWriteLine(debug, str);
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired.
        /// </summary>
        /// <param name="timeout"></param>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout) => Console.ReadKeyEx(false, timeout, false);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. Uses the given foreground color.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, Color forecolor)
            => Console.ReadKeyEx(false, timeout, false, forecolor);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. Uses the given foreground and background colors.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            TimeSpan timeout, Color forecolor, Color backcolor)
            => Console.ReadKeyEx(false, timeout, false, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool hide)
            => Console.ReadKeyEx(false, timeout, hide);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested,
        /// using the given foreground color.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool hide, Color forecolor)
            => Console.ReadKeyEx(false, timeout, hide, forecolor);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested,
        /// using the given foreground and background colors.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            TimeSpan timeout, bool hide, Color forecolor, Color backcolor)
            => Console.ReadKeyEx(false, timeout, hide, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console, and to the debug environment if requested, .
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout)
            => Console.ReadKeyEx(debug, timeout, false);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console, and to the debug environment if requested, the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, Color forecolor)
            => Console.ReadKeyEx(debug, timeout, false, forecolor);

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console, and to the debug environment if requested, using the given foreground
        /// and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, Color forecolor, Color backcolor)
            => Console.ReadKeyEx(debug, timeout, false, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout, bool hide)
        {
            var ini = DateTime.UtcNow;
            var ms = timeout.ValidatedTimeout;

            while (true)
            {
                // Trying an available key...
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(hide);
                    TryDebugWriteLine(debug, ToHumanString(info));
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
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested,
        /// using the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, bool hide, Color forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(debug, timeout, hide);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// Obtains the next character or function key pressed by the user. Returns <c>null</c>
        /// if the timeout period has expired. A string representation of the that key is written
        /// in the console if not hide is requested, and to the debug environment if requested,
        /// using the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="hide"></param>
        /// <param name="forecolor"></param>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, bool hide, Color forecolor, Color backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(debug, timeout, hide);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may be
        /// returned.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(string? source) => Console.EditLineEx(false, source);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may be
        /// returned. Uses the given foreground color.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, Color forecolor) => Console.EditLineEx(false, source, forecolor);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may be
        /// returned. Uses the given foreground and background colors.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, Color forecolor, Color backcolor)
            => Console.EditLineEx(false, source, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout)
            => Console.EditLineEx(false, source, timeout);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned. Uses the given foreground color.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout, Color forecolor)
            => Console.EditLineEx(false, source, timeout, forecolor);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned. Uses the given foreground and background colors.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLine(
            string? source, TimeSpan timeout, Color forecolor, Color backcolor)
            => Console.EditLineEx(false, source, timeout, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may
        /// be returned. If such is requested, the result is written to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source)
            => Console.EditLineEx(debug, source, Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may be
        /// returned. If such is requested, the result is written to the debug environment. Uses
        /// the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, Color forecolor)
            => Console.EditLineEx(debug, source, Timeout.InfiniteTimeSpan, forecolor);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled; otherwise, an empty one may be
        /// returned. If such is requested, the result is written to the debug environment. Uses
        /// the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, Color forecolor, Color backcolor)
            => Console.EditLineEx(debug, source, Timeout.InfiniteTimeSpan, forecolor, backcolor);

        // ------------------------------------------------

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned. If such is requested, the result is written to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, TimeSpan timeout)
            => Console.EditLineEx(debug, source, timeout, Console.ForegroundColor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned. If such is requested, the result is written to the debug
        /// environment. Uses the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, TimeSpan timeout, Color forecolor)
            => Console.EditLineEx(debug, source, timeout, forecolor, Console.BackgroundColor);

        /// <summary>
        /// Edits in the console the given source string (or an empty initial one if it was null).
        /// Returns <c>null</c> if such edition was cancelled or the timeout expired; otherwise,
        /// an empty one may be returned. If such is requested, the result is written to the debug
        /// environment. Uses the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, string? source, TimeSpan timeout, Color forecolor, Color backcolor)
        {
            // TODO: EditLineEx needs to guarantee window buffer is big enough.

            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var oldsize = Console.CursorSize;
            int oldleft = Console.CursorLeft;

            var sb = new StringBuilder(source ?? string.Empty);
            var pos = sb.Length;
            bool insert = IsInInsertMode(raise: false);
            int len;

            SetInsertMode(false);
            ShowLine(pos);

            while (true)
            {
                // Capturing, or ESC if timeout...
                var info = ReadKey(timeout, hide: true);
                info ??= new('\0', ConsoleKey.Escape, false, false, false);

                // Special keys...
                switch (info.Value.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        SetInsertMode(false);
                        Console.ForegroundColor = oldfore;
                        Console.BackgroundColor = oldback;
                        TryDebugWriteLine(debug, sb.ToString());
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
            /// Invoked to show the current value, clearing up to the remaining len. Then, sets
            /// the cursor to the given position.
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
            /// Sets the insert mode to the given value. This method is only supported in a
            /// windows environment.
            /// </summary>
            void SetInsertMode(bool value)
            {
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (windows)
                {
                    Console.CursorSize = value ? 100 : oldsize;
                    insert = value;
                }
            }
        }
    }
}