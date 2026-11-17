using Color = System.ConsoleColor;
namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// In windows environments, makes room in the console buffer to prevent buffer overflow
        /// conditions to happen when using Cursor-alike and Buffer-alike properties, and similar
        /// methods.
        /// </summary>
        /// <param name="num">The minimum number of lines to make room for.</param>
        /// <param name="keep">The minimum percentage of lines to keep.</param>
        public static void PreventBufferOverflow(int num, int keep = 50)
        {
            if (!OperatingSystem.IsWindows()) return;

            var bheight = Console.BufferHeight;
            var bwidth = Console.BufferWidth;
            var top = Console.CursorTop;
            if ((top + num) < bheight) return;

            if (keep < 1) throw new ArgumentOutOfRangeException(nameof(keep)).WithData(keep);
            if (keep > 100) throw new ArgumentOutOfRangeException(nameof(keep)).WithData(keep);
            var percentage = keep / 100.0;
            var lines = (int)Math.Ceiling(Console.BufferHeight * percentage);
            if (lines < num) lines = num;

            var orig = top - lines;
            if (orig < 0) return;

            Console.MoveBufferArea(0, orig, bwidth, lines, 0, 0);
            Console.SetWindowPosition(0, 0);
            Console.SetCursorPosition(0, lines);
        }
    }

    // ------------------------------------------------

    /// <summary>
    /// If debug is requested, writes the given message in the not console-alike listeners.
    /// </summary>
    [Conditional("DEBUG")]
    internal static void TryDebugWrite(bool debug, string message)
    {
        if (!debug) return;

        foreach (TraceListener item in Trace.Listeners)
            if (!item.IsConsoleListener) item.Write(message);
    }

    /// <summary>
    /// If debug is requested, writes the given message in the not console-alike listeners,
    /// followed by the current line terminator.
    /// </summary>
    [Conditional("DEBUG")]
    internal static void TryDebugWriteLine(bool debug, string message)
    {
        if (!debug) return;

        foreach (TraceListener item in Trace.Listeners)
            if (!item.IsConsoleListener) item.WriteLine(message);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a formatted message.
    /// </summary>
    internal static string FormatMessage(string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        return message.Length != 0 && args.Length != 0 ? string.Format(message, args) : message;
    }

    /// <summary>
    /// Obtains a string with the given number of spaces.
    /// </summary>
    [SuppressMessage("", "CA2208")]
    internal static string FromSpaces(int num)
    {
        if (num < 0) throw new ArgumentOutOfRangeException("Negative number of spaces.").WithData(num);
        if (num == 0) return string.Empty;

        lock (Headers)
        {
            if (Headers.TryGetValue(num, out var value)) return value;
            Headers.Add(num, value = new(' ', num));
            return value;
        }
    }
    readonly static Dictionary<int, string> Headers = [];

    // ----------------------------------------------------

    /// <summary>
    /// Returns a human description of the key or character.
    /// </summary>
    internal static string ToHumanString(ConsoleKeyInfo info)
    {
        return info.KeyChar < 32
            ? $"[{info.Key}]"
            : $"{info.KeyChar}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains an alternate color with a soft variation based upon the given one.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    internal static Color AlternateColor(Color color) => color switch
    {
        // Light colors...
        Color.White => Color.Gray,
        Color.Gray => Color.DarkGray,
        Color.DarkGray => Color.Black,
        Color.Yellow => Color.DarkYellow,
        Color.Cyan => Color.DarkCyan,
        Color.Magenta => Color.DarkMagenta,
        Color.Red => Color.DarkRed,
        Color.Green => Color.DarkGreen,
        Color.Blue => Color.DarkBlue,

        // Dark colors...
        Color.Black => Color.Gray,
        Color.DarkYellow => Color.Yellow,
        Color.DarkCyan => Color.Cyan,
        Color.DarkMagenta => Color.Magenta,
        Color.DarkRed => Color.Red,
        Color.DarkGreen => Color.Green,
        Color.DarkBlue => Color.Blue,

        // Default...
        _ => color
    };
}