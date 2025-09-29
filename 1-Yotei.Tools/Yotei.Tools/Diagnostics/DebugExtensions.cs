namespace Yotei.Tools.Diagnostics;

// =============================================================
public static class DebugExtensions
{
    extension(Debug)
    {
        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteEx(
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Debug.Write(message);
        }

        // -----------------------------------------------------

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, and optionally
        /// replicates it to the console (provided there is no console-alike listener registered).
        /// </summary>
        /// <param name="console"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteEx(
            bool console,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);
            
            WriteEx(message);

            if (console)
            {
                if (Ambient.IsConsoleListener()) return;

                var pos = Console.CursorLeft;
                if (pos <= 1)
                {
                    var size = Debug.IndentSize * Debug.IndentLevel;
                    var header = Header(size);
                    Console.Write(header);
                }
                Console.Write(message);
            }
        }

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, and optionally
        /// replicates it to the console (provided there is no console-alike listener registered),
        /// using the given foreground color.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteEx(
            bool console,
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteEx(console, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, and optionally
        /// replicates it to the console (provided there is no console-alike listener registered),
        /// using the given foreground and background colors.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteEx(
            bool console,
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteEx(console, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, followed by the
        /// current line terminator.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineEx(
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, followed by the
        /// current line terminator, and optionally replicates it to the console (provided there is
        /// no console-alike listener registered).
        /// </summary>
        /// <param name="console"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineEx(
            bool console,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            message ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) message = string.Format(message, args);

            WriteLineEx(message);

            if (console)
            {
                if (Ambient.IsConsoleListener()) return;

                var pos = Console.CursorLeft;
                if (pos <= 1)
                {
                    var size = Debug.IndentSize * Debug.IndentLevel;
                    var header = Header(size);
                    Console.Write(header);
                }
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, followed by the
        /// current line terminator, and optionally replicates it to the console (provided there is
        /// no console-alike listener registered), using the given foreground color.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="forecolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineEx(
            bool console,
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            WriteLineEx(console, message, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the given message, formatted using the given arguments, if any, followed by the
        /// current line terminator, and optionally replicates it to the console (provided there is
        /// no console-alike listener registered), using the given foreground and background colors.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineEx(
            bool console,
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            WriteLineEx(console, message, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    internal static string Header(int size)
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

        if (!Headers.TryGetValue(size, out var header)) Headers.Add(size, header = new(Space, size));
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