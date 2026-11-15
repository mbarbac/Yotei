using System.Runtime.InteropServices.Marshalling;
using Color = System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
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
    
    const int STD_INPUT_HANDLE = -10;
    const uint ENABLE_INSERT_MODE = 0x0020;

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    /// <summary>
    /// Used to determine if the console was in insert mode.
    /// </summary>
    internal static bool IsInInsertMode(bool raise = false)
    {
        var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        if (!windows) return false;

        IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        if (GetConsoleMode(consoleHandle, out uint consoleMode))
            return (consoleMode & ENABLE_INSERT_MODE) != 0;

        return raise
            ? throw new InvalidOperationException("Unable to retrieve console insert mode. Ensure output/input is not redirected.")
            : false;
    }
}