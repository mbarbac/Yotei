namespace Yotei.Tools;

// =============================================================
public static class ConsoleExtensions
{
    /// <summary>
    /// Executes the given action in an ambient with no console listeners.
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

    // ---------------------------------------------------------

    extension(Console)
    {
        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Uses the given foreground and background colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Console.Write(message);
            if (debug) WithNoConsoleListeners(() => Debug.Write(message));
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug,
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// Uses the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteEx(
            bool debug,
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteEx(debug, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// Uses the given foreground and background colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Console.WriteLine(message);
            if (debug) WithNoConsoleListeners(() => Debug.WriteLine(message));
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug,
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteLineEx(debug, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string, object?[]?)"/>
        /// Replicates the message on the debug environment if requested.
        /// Uses the given foreground and background colors.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLineEx(
            bool debug,
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteLineEx(debug, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(ConsoleColor forecolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var str = Console.ReadLine();
            Console.ForegroundColor = oldfore;
            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Uses the given foreground and background colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = Console.ReadLine();
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
            return str;
        }

        // -----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// If requested, duplicates the result in the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string? ReadLineEx(bool debug)
        {
            var str = Console.ReadLine();
            if (str is not null && debug) WithNoConsoleListeners(() => Debug.WriteLine(str));

            return str;
        }

        /// <summary>
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Uses the given foreground color.
        /// If requested, duplicates the result in the debug environment.
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
        /// <inheritdoc cref="Console.ReadLine"/>
        /// Uses the given foreground and background colors.
        /// If requested, duplicates the result in the debug environment.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public static string? ReadLineEx(
            bool debug,
            ConsoleColor forecolor, ConsoleColor backcolor)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            var str = ReadLineEx(debug);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
            return str;
        }

        // -----------------------------------------------------

        /// <summary>
        /// Returns the next character or function key pressed by the user, or '<c>null</c>' if
        /// the given timeout elapses. If <paramref name="intercept"/> is 'false' then the pressed
        /// key, if any, is displayed; if it is 'true', then the pressed key is intercepted and
        /// not displayed.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            TimeSpan timeout, bool intercept = false) => ReadKey(false, timeout, intercept);

        /// <summary>
        /// Returns the next character or function key pressed by the user, or '<c>null</c>' if
        /// the given timeout elapses. The pressed key, if any, is displayed. Replicates the
        /// result in the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(bool debug, TimeSpan timeout)
            => ReadKey(debug, timeout, intercept: false);

        /// <summary>
        /// Returns the next character or function key pressed by the user.
        /// If <paramref name="intercept"/> is 'false' then the pressed key, if any, is displayed;
        /// if it is 'true', then the key is intercepted and not displayed. Replicates the result
        /// in the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(
            bool debug, bool intercept = false)
            => ReadKey(debug, Timeout.InfiniteTimeSpan, intercept);

        /// <summary>
        /// Returns the next character or function key pressed by the user, or '<c>null</c>' if
        /// the given timeout elapses. If <paramref name="intercept"/> is 'false' then the pressed
        /// key, if any, is displayed; if it is 'true', then the pressed key is intercepted and
        /// not displayed. Replicates the result in the debug environment if requested.
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="timeout"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static ConsoleKeyInfo? ReadKey(bool debug, TimeSpan timeout, bool intercept = false)
        {
            var ms = timeout.ValidatedTimeout;
            var ini = DateTime.UtcNow;

            while (true)
            {
                // When a key is available...
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(true); // Preventing display, we take care...
                    if (!intercept)
                    {
                        var ch = info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";
                        Console.Write(ch);
                        if (debug) WithNoConsoleListeners(() => Debug.Write(ch));
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
                Thread.Sleep(10);
            }
        }
    }
}