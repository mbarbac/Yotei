namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class DebugExtensions
{
    extension(Debug)
    {
        /// <summary>
        /// Writes using the specified format the text representation of the specified array of
        /// objects to the registered trace listeners and, if requested, to the standard output
        /// stream.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(
            bool console,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
            params object?[]? args)
        {
            format ??= string.Empty;
            args ??= [null];
            if (args.Length > 0) format = string.Format(format, args);

            // TODO: prevent write on console if there is a console-alike trace listener.

            Console.Write(format);
            Debug.Write(format);
        }

        /// <summary>
        /// Writes using the specified format the text representation of the specified array of
        /// objects to the registered trace listeners and to the standard output stream, using
        /// the given foreground color.
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
            Write(true, format, args);
            Console.ForegroundColor = oldfore;
        }
    }
}