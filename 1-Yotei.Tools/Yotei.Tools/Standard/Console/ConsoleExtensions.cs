using Color = System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
public static class ConsoleExtensions
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
    }
}