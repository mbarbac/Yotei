namespace Yotei.Tools;

// =============================================================
public static class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// <inheritdoc cref="Console.Write(string, object?[]?)"/>
        /// Uses the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Write(
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
        public static void Write(
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

            // To console: only if requested, preventing the usage of console-alike listeners...
            if (debug)
            {
                var exist = Ambient.IsConsoleListener();
                var items = exist ? Ambient.GetConsoleListeners().ToArray() : [];
                if (exist) Ambient.RemoveListeners(items);

                Debug.Write(message);
                if (exist) Ambient.AddListeners(items);
            }
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
            Console.WriteEx(debug, message, args);
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
            Console.WriteEx(debug, message, args);
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
        public static void WriteLine(
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
        public static void WriteLine(
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

            // To console: only if requested, preventing the usage of console-alike listeners...
            if (debug)
            {
                var exist = Ambient.IsConsoleListener();
                var items = exist ? Ambient.GetConsoleListeners().ToArray() : [];
                if (exist) Ambient.RemoveListeners(items);

                Debug.WriteLine(message);
                if (exist) Ambient.AddListeners(items);
            }
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
            Console.WriteLineEx(debug, message, args);
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
            Console.WriteLine(message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }
    }
}