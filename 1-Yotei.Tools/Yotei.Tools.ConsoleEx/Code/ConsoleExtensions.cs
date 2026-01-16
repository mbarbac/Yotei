namespace Yotei.Tools;

// ========================================================
public static class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void Write(
            ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(Format(message, args));
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void Write(
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(Format(message, args));
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteEx(bool debug, string message, params object?[]? args)
        {
            var str = Format(message, args);
            Console.Write(str);
            if (DoDebug(debug)) Debug.Write(str);
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteEx(
            bool debug,
            ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteEx(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteEx(debug, message, args);
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
        /// <returns></returns>
        public static void WriteLine(
            ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(Format(message, args));
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteLine(
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(Format(message, args));
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine()"/> In addition, if requested and if there are no
        /// console listeners registered, sends the formatted message to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        public static void WriteLineEx(bool debug)
        {
            Console.WriteLine();
            if (DoDebug(debug)) Debug.WriteLine(string.Empty);
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteLineEx(bool debug, string message, params object?[]? args)
        {
            var str = Format(message, args);
            Console.WriteLine(str);
            if (DoDebug(debug)) Debug.WriteLine(str);
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteLineEx(
            bool debug,
            ConsoleColor forecolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteLineEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/> In addition, if requested and
        /// if there are no console listeners registered, sends the formatted message to the debug
        /// environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void WriteLineEx(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteLineEx(debug, message, args);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// </summary>
        /// <param name="forecolor"></param>
        /// <returns>The next line read from the input stream, or null if any.</returns>
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
        /// <returns>The next line read from the input stream, or null if any.</returns>
        public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor)
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
        /// Returns the next line of characters read from the input stream, or <see langword="null"/>
        /// if no line was available. In addition, if requested and if there are no console listeners
        /// registered, sends the formatted message to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug)
        {
            var str = Console.ReadLine();
            if (str is not null && DoDebug(debug)) Debug.WriteLine(str);
            return str;
        }

        /// <summary>
        /// Returns the next line of characters read from the input stream, or <see langword="null"/>
        /// if no line was available. In addition, if requested and if there are no console listeners
        /// registered, sends the formatted message to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = ReadLineEx(debug);
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// Returns the next line of characters read from the input stream, or <see langword="null"/>
        /// if no line was available. In addition, if requested and if there are no console listeners
        /// registered, sends the formatted message to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = ReadLineEx(debug);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout) => ReadKey(timeout, false);

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKey(timeout);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKey(timeout);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>).
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool intercept)
        {
            var ini = DateTime.UtcNow;
            var ms = ValidatedTimeOut(timeout);

            while (true)
            {
                // Trying an available key---
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(intercept);
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
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>).
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, bool intercept, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKey(timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>).
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool intercept, ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKey(timeout, intercept);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// In addition, if requested and if there are no console listeners registered, sends the key
        /// representation to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout)
        {
            var info = ReadKey(timeout);
            if (info != null && DoDebug(debug)) Debug.Write(info.Value);
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// In addition, if requested and if there are no console listeners registered, sends the key
        /// representation to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(debug, timeout);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is displayed in the console window.
        /// In addition, if requested and if there are no console listeners registered, sends the key
        /// representation to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var info = ReadKeyEx(debug, timeout);
            Console.BackgroundColor = oldback;
            Console.ForegroundColor = oldfore;
            return info;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>). In addition, if
        /// requested and if there are no console listeners registered, sends the key representation
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(bool debug, TimeSpan timeout, bool intercept)
        {
            var info = ReadKey(timeout, intercept);
            if (info != null && DoDebug(debug)) Debug.Write(ToHumanString(info.Value));
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>). In addition, if
        /// requested and if there are no console listeners registered, sends the key representation
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug, TimeSpan timeout, bool intercept, ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var info = ReadKeyEx(debug, timeout, intercept);
            Console.ForegroundColor = oldfore;
            return info;
        }

        /// <summary>
        /// Returns either the next character or function key pressed by the user, or <see langword="null"/>
        /// if the timeout period expired. The pressed key, if any, is optionally displayed in the
        /// console window (<paramref name="intercept"/>: <see langword="false"/>). In addition, if
        /// requested and if there are no console listeners registered, sends the key representation
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKeyEx(
            bool debug,
            TimeSpan timeout, bool intercept, ConsoleColor forecolor, ConsoleColor backcolor)
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
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(string? source = null)
        {
            return EditLineEx(
                false,
                Timeout.InfiniteTimeSpan,
                Console.ForegroundColor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]).
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(ConsoleColor forecolor, string? source = null)
        {
            return EditLineEx(
                false,
                Timeout.InfiniteTimeSpan,
                forecolor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]).
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        {
            return EditLineEx(
                false,
                Timeout.InfiniteTimeSpan,
                forecolor,
                backcolor,
                source);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(TimeSpan timeout, string? source = null)
        {
            return EditLineEx(
                false,
                timeout,
                Console.ForegroundColor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            TimeSpan timeout, ConsoleColor forecolor, string? source = null)
        {
            return EditLineEx(
                false,
                timeout,
                forecolor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLine(
            TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        {
            return EditLineEx(
                false,
                timeout,
                forecolor,
                backcolor,
                source);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]). In addition, if not null and if
        /// there are no console listeners registered, sends the string to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(bool debug, string? source = null)
        {
            return EditLineEx(
                debug,
                Timeout.InfiniteTimeSpan,
                Console.ForegroundColor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]). In addition, if not null and if
        /// there are no console listeners registered, sends the string to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(bool debug, ConsoleColor forecolor, string? source = null)
        {
            return EditLineEx(
                debug,
                Timeout.InfiniteTimeSpan,
                forecolor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]). In addition, if not null and if
        /// there are no console listeners registered, sends the string to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        {
            return EditLineEx(
                debug,
                Timeout.InfiniteTimeSpan,
                forecolor,
                backcolor,
                source);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired. In
        /// addition, if not null and if there are no console listeners registered, sends the string
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx( bool debug, TimeSpan timeout, string? source = null)
        {
            return EditLineEx(
                debug,
                timeout,
                Console.ForegroundColor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired. In
        /// addition, if not null and if there are no console listeners registered, sends the string
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug, TimeSpan timeout, ConsoleColor forecolor, string? source = null)
        {
            return EditLineEx(
                debug,
                timeout,
                forecolor,
                Console.BackgroundColor,
                source);
        }

        /// <summary>
        /// Returns the string editted by the user in the console window, or <see langword="null"/>
        /// if that edition was cancelled (by pressing [Escape]) or if the timeout period expired. In
        /// addition, if not null and if there are no console listeners registered, sends the string
        /// to the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string? EditLineEx(
            bool debug,
            TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor,
            string? source = null)
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    // Obtains a formatted message.
    static string Format(string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        return message.Length != 0 && args.Length != 0 ? string.Format(message, args) : message;
    }

    // Determines if we shall also write in the debug environment, or not.
    static bool DoDebug(bool debug) => debug && !Trace.Listeners.ConsoleListeners.Any();

    // Obtains a human representation of the given console info value.
    static string ToHumanString(ConsoleKeyInfo info)
        => info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";

    // Obtains a validated timeout value, in milliseconds.
    static long ValidatedTimeOut(TimeSpan source)
    {
        var ms = (long)source.TotalMilliseconds;
        if (ms < -1 || ms > int.MaxValue) throw new ArgumentOutOfRangeException(
            nameof(source),
            $"Invalid timeout value: {ms}");

        return ms;
    }

    // Returns a string with the given number of spaces.
    static string FromSpaces(int num)
    {
        switch (num)
        {
            case 0: return Header0;
            case 1: return Header1;
            case 2: return Header2;
            case 3: return Header3;
            case 4: return Header4;
            case 8: return Header8;
        }
        if (!Headers.TryGetValue(num, out var header)) Headers.Add(num, header = new(' ', num));
        return header;
    }
    readonly static Dictionary<int, string> Headers = [];
    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(' ', 1);
    readonly static string Header2 = new(' ', 2);
    readonly static string Header3 = new(' ', 3);
    readonly static string Header4 = new(' ', 4);
    readonly static string Header8 = new(' ', 8);
}