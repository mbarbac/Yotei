namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Parses into dot-separated member names the given dynamic lambda expression.
/// </summary>
public class LambdaNameParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into. If all parts are empty, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string Parse(Func<dynamic, object> expression) => Parse(expression, out _, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns in the out arguments the collection of parts names. If all
    /// parts are empty, then returns an empty string, and an empty array of parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out string[] parts)
        => Parse(expression, out parts, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns the  dynamic argument used for the expression. If all parts
    /// are empty, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out LambdaNodeArgument arg)
        => Parse(expression, out _, out arg);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns in the out arguments the collection of parts names, and the
    /// dynamic argument used for the expression. If all parts are empty, then returns an empty
    /// string, and an empty array of parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out string[] parts, out LambdaNodeArgument arg)
    {
        expression.ThrowWhenNull();

        var parser = LambdaParser.Parse(expression);
        if (parser.DynamicArguments.Length != 1) throw new ArgumentException(
            "Dynamic lambda name parser requires one and only one dynamic argument.")
            .WithData(parser, nameof(expression));

        var named = new LambdaNameParser();
        named.Parser = parser;
        named.Argument = arg = parser.DynamicArguments[0];

        named.OnParse(parser.Result, true);

        parts = named.List.ToArray();
        return parts.All(x => x.Length == 0) ? string.Empty : string.Join('.', parts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Private constructor.
    /// </summary>
    private LambdaNameParser() { }

    /// <summary>
    /// The parser used by this instance.
    /// </summary>
    LambdaParser Parser { get; set; } = default!;

    /// <summary>
    /// The dynamic argument used to invoked the expression parsed.
    /// </summary>
    LambdaNodeArgument Argument { get; set; } = default!;

    /// <summary>
    /// The list of parsed parts.
    /// </summary>
    List<string> List { get; } = [];

    // ----------------------------------------------------l
}