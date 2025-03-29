namespace Yotei.Tools;

// ========================================================
internal static class LambdaDebug
{
    internal const ConsoleColor NewNodeColor = ConsoleColor.White;
    internal const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Blue;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.Yellow;

    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(string message) => DebugEx.WriteLine(message);

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(color, message);
}