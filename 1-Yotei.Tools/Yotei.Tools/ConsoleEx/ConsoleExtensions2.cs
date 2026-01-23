namespace Yotei.Tools;

// ========================================================
public static partial class ConsoleExtensions
{
    /// <summary>
    /// Writes the given message in the no-console-alike listeners, if debug is requested.
    /// </summary>
    [Conditional("DEBUG")]
    internal static void TryDebugWrite(bool debug, string message)
    {
        if (debug)
        {
            foreach (TraceListener item in Trace.Listeners)
                if (!item.IsConsoleListener) item.Write(message);
        }
    }

    /// <summary>
    /// Writes the given message in the no-console-alike listeners followed by a line terminator,
    /// if debug is requested.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    [Conditional("DEBUG")]
    internal static void TryDebugWriteLine(bool debug, string message)
    {
        if (debug)
        {
            foreach (TraceListener item in Trace.Listeners)
                if (!item.IsConsoleListener) item.WriteLine(message);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a human description of the key or character.
    /// </summary>
    static string ToHumanString(ConsoleKeyInfo info)
        => info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a formatted message.
    /// </summary>
    static string FormatMessage(string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        return message.Length != 0 && args.Length != 0 ? string.Format(message, args) : message;
    }

    /// <summary>
    /// Obtains a string with the given number of spaces.
    /// </summary>
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