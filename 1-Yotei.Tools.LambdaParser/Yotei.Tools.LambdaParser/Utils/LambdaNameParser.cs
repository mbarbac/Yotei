namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Parses into dot-separated parts the names the given dynamic lambda expression resolves into.
/// </summary>
public static class LambdaNameParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated parts of the names that
    /// expression resolves into, and returns combined name specification. If all parts are empty
    /// ones, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression) => Parse(expression, out _, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated parts of the names that
    /// expression resolves into, and returns both the combined name specification and its parts.
    /// If all parts are empty ones, then returns an empty string and an empty array of parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out string[] parts)
        => Parse(expression, out parts, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated parts of the names that
    /// expression resolves into, and returns the combined name specification. If all parts are
    /// empty ones, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out LambdaNodeArgument arg)
        => Parse(expression, out _, out arg);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated parts of the names that
    /// expression resolves into, and returns both the combined name specification and its parts.
    /// If all parts are empty ones, then returns an empty string and an empty array of parts.
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

        var items = new List<string>();
        OnParse(parser.Result, items, parser);

        parts = [.. items];
        arg = parser.DynamicArguments[0];

        return parts.Length == 0 || parts.All(x => x.Length == 0)
            ? string.Empty
            : string.Join(".", parts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to recursively parse the given node.
    /// </summary>
    static void OnParse(LambdaNode node, List<string> items, LambdaParser parser)
    {
        switch (node)
        {
            case LambdaNodeArgument: break;

            case LambdaNodeMember item: OnParseMember(item, items, parser); break;
            case LambdaNodeIndexed item: OnParseIndexed(item, items, parser); break;
            case LambdaNodeInvoke item: OnParseInvoke(item, items, parser); break;
            case LambdaNodeValue item: OnParseValue(item, items, parser); break;

            default:
                throw new ArgumentException(
                    "Expression carries a not-supported node.")
                    .WithData(node)
                    .WithData(parser, "expression");
        }
    }

    /// <summary>
    /// Invoked to parse the given member node.
    /// </summary>
    static void OnParseMember(LambdaNodeMember node, List<string> items, LambdaParser parser)
    {
        OnParse(node.LambdaHost, items, parser);

        var arg = parser.DynamicArguments[0];
        var name = node.LambdaName == arg.LambdaName
            ? string.Empty
            : node.LambdaName;

        items.Add(name);
    }

    /// <summary>
    /// Invoked to parse the given indexed node.
    /// </summary>
    static void OnParseIndexed(LambdaNodeIndexed node, List<string> items, LambdaParser parser)
    {
        if (node.LambdaIndexes.Length != 1)
            throw new ArgumentException(
                "Indexed parts require one and only one index.")
                .WithData(node)
                .WithData(parser, "expression");

        OnParse(node.LambdaHost, items, parser);

        var temps = new List<string>();
        OnParse(node.LambdaIndexes[0], temps, parser);

        if (items.Count > 0) items[^1] += temps[0];
        else items.Add(temps[0]);

        for (int i = 1; i < temps.Count; i++) items.Add(temps[i]);
    }

    /// <summary>
    /// Invoked to parse the given indexed node.
    /// </summary>
    static void OnParseInvoke(LambdaNodeInvoke node, List<string> items, LambdaParser parser)
    {
        if (node.LambdaArguments.Length != 1)
            throw new ArgumentException(
                "Invoke parts require one and only one argument.")
                .WithData(node)
                .WithData(parser, "expression");

        OnParse(node.LambdaHost, items, parser);

        var temps = new List<string>();
        OnParse(node.LambdaArguments[0], temps, parser);

        if (items.Count > 0) items[^1] += temps[0];
        else items.Add(temps[0]);

        for (int i = 1; i < temps.Count; i++) items.Add(temps[i]);
    }

    /// <summary>
    /// Invoked to parse the constant value node.
    /// </summary>
    static void OnParseValue(LambdaNodeValue node, List<string> items, LambdaParser parser)
    {
        if (node.LambdaValue == null)
        {
            items.Add(string.Empty);
        }
        else if (node.LambdaValue is string str)
        {
            var names = str.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i].NullWhenEmpty(trim: true) ?? string.Empty;
                items.Add(name);
            }
        }
        else if (node.LambdaValue.GetType().IsPrimitive)
        {
            var name = node.LambdaValue.ToString() ?? string.Empty;
            items.Add(name);
        }
        else
        {
            throw new ArgumentException(
                "Expression carries a not supported constant.")
                .WithData(node)
                .WithData(parser, "expression");
        }
    }
}