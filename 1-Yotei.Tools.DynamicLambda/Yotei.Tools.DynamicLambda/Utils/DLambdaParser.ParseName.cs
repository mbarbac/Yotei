namespace Yotei.Tools.DynamicLambda;

// ========================================================
public partial class DLambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns the name that expression resolves
    /// into. If all its parts are empty ones, then returns an empty string. A part is an empty
    /// one if it resolves to the name of the dynamic argument.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression) => ParseName(expression, out _, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression and returns the name that expression resolves
    /// into. If all its parts are empty ones, then returns an empty string. A part is an empty
    /// one if it resolves to the name of the dynamic argument. This method also returns in the
    /// out argument the collection of dot-separated name parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression, out string[] parts)
        => ParseName(expression, out parts, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression and returns the name that expression resolves
    /// into. If all its parts are empty ones, then returns an empty string. A part is an empty
    /// one if it resolves to the name of the dynamic argument. This method also returns in the
    /// out argument the dynamic argument used to invoke the expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression, out DLambdaNodeArgument arg)
        => ParseName(expression, out _, out arg);

    /// <summary>
    /// Parses the given dynamic lambda expression and returns the name that expression resolves
    /// into. If all its parts are empty ones, then returns an empty string. A part is an empty
    /// one if it resolves to the name of the dynamic argument. This method also returns in the
    /// out arguments the collection of dot-separated name parts, and the dynamic argument used
    /// to invoke the expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression, out string[] parts, out DLambdaNodeArgument arg)
    {
        expression.ThrowWhenNull();

        var parser = Parse(expression);
        if (parser.DynamicArguments.Length != 1) throw new ArgumentException(
            "Dynamic lambda name parser requires one and only one dynamic argument.")
            .WithData(parser, nameof(expression));

        var items = new List<string>();
        OnParseName(parser.Result, items, parser);

        parts = [.. items];
        arg = parser.DynamicArguments[0];

        return parts.Length == 0 || parts.All(static x => x.Length == 0)
            ? string.Empty
            : string.Join(".", parts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to recursively parse the name of the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="items"></param>
    /// <param name="parser"></param>
    static void OnParseName(DLambdaNode node, List<string> items, DLambdaParser parser)
    {
        switch (node)
        {
            case DLambdaNodeArgument: break;

            case DLambdaNodeMember item: OnParseNameMember(item, items, parser); break;
            case DLambdaNodeIndexed item: OnParseNameIndexed(item, items, parser); break;
            case DLambdaNodeInvoke item: OnParseNameInvoke(item, items, parser); break;
            case DLambdaNodeValue item: OnParseNameValue(item, items, parser); break;

            default:
                throw new ArgumentException(
                    "Expression carries a not-supported node.")
                    .WithData(node)
                    .WithData(parser, "expression");
        }
    }

    /// <summary>
    /// Invoked to parse the name of the given element.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="items"></param>
    /// <param name="parser"></param>
    static void OnParseNameMember(DLambdaNodeMember node, List<string> items, DLambdaParser parser)
    {
        OnParseName(node.DLambdaHost, items, parser);

        var arg = parser.DynamicArguments[0];
        var name = node.DLambdaName == arg.DLambdaName
            ? string.Empty
            : node.DLambdaName;

        items.Add(name);
    }

    /// <summary>
    /// Invoked to parse the name of the given element.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="items"></param>
    /// <param name="parser"></param>
    static void OnParseNameIndexed(DLambdaNodeIndexed node, List<string> items, DLambdaParser parser)
    {
        if (node.DLambdaIndexes.Length != 1)
            throw new ArgumentException(
                "Indexed parts require one and only one index.")
                .WithData(node)
                .WithData(parser, "expression");

        OnParseName(node.DLambdaHost, items, parser);

        var temps = new List<string>();
        OnParseName(node.DLambdaIndexes[0], temps, parser);

        if (items.Count > 0) items[^1] += temps[0];
        else items.Add(temps[0]);

        for (int i = 1; i < temps.Count; i++) items.Add(temps[i]);
    }

    /// <summary>
    /// Invoked to parse the name of the given element.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="items"></param>
    /// <param name="parser"></param>
    static void OnParseNameInvoke(DLambdaNodeInvoke node, List<string> items, DLambdaParser parser)
    {
        if (node.DLambdaArguments.Length != 1)
            throw new ArgumentException(
                "Invoke parts require one and only one argument.")
                .WithData(node)
                .WithData(parser, "expression");

        OnParseName(node.DLambdaHost, items, parser);

        var temps = new List<string>();
        OnParseName(node.DLambdaArguments[0], temps, parser);

        if (items.Count > 0) items[^1] += temps[0];
        else items.Add(temps[0]);

        for (int i = 1; i < temps.Count; i++) items.Add(temps[i]);
    }

    /// <summary>
    /// Invoked to parse the name of the given element.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="items"></param>
    /// <param name="parser"></param>
    static void OnParseNameValue(DLambdaNodeValue node, List<string> items, DLambdaParser parser)
    {
        if (node.DLambdaValue == null)
        {
            items.Add(string.Empty);
        }
        else if (node.DLambdaValue is string str)
        {
            var names = str.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i].NullWhenEmpty(trim: true) ?? string.Empty;
                items.Add(name);
            }
        }
        else if (node.DLambdaValue.GetType().IsPrimitive)
        {
            var name = node.DLambdaValue.ToString() ?? string.Empty;
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