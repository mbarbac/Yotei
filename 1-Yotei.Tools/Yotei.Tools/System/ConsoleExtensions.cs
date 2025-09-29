namespace Yotei.Tools;

// =============================================================
public static class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// Writes the text representation of the specified array of objects to the standard output
        /// stream using the specified format information and the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.Write(format, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects to the standard output
        /// stream using the specified format information and the given foreground and background
        /// colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.Write(format, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }

        // -----------------------------------------------------

        /// <summary>
        /// Writes the text representation of the specified array of objects to the standard output
        /// stream, followed by the current line terminator, using the specified format information
        /// and the given foreground color.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            ConsoleColor forecolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            Console.WriteLine(format, args);
            Console.ForegroundColor = oldfore;
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects to the standard output
        /// stream, followed by the current line terminator, using the specified format information
        /// and the given foreground and background colors.
        /// </summary>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(
            ConsoleColor forecolor,
            ConsoleColor backcolor,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
            params object?[]? args)
        {
            var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
            var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
            Console.WriteLine(format, args);
            Console.ForegroundColor = oldfore;
            Console.BackgroundColor = oldback;
        }
    }
}