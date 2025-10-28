namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions. Instances of this class contain
/// the last node of the chain of dynamic operations binded against the dynamic arguments of that
/// expression, along with that dynamic arguments arguments.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaParser
{
    // ----------------------------------------------------

    internal const ConsoleColor NewNodeColor = ConsoleColor.White;
    internal const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Yellow;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.Blue;
    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(ConsoleColor color, string message)
    {
        var old = Console.ForegroundColor; Console.ForegroundColor = color;
        Debug.WriteLine(message);
        Console.ForegroundColor = old;
    }
}