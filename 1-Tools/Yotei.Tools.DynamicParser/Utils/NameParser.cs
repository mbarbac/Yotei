namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides the ability of parsing dynamic lambda expressions, returning the chain or names
/// they resolve into.
/// </summary>
public static class NameParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression returning the multipart name it resolves
    /// into, where these parts are joined by dots. If any part is null or empty (including
    /// those whose name resolve into the name of the dynamic argument), then it is parsed as
    /// an empty literal. If all parts are null or empty, then an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string? Parse(
        Func<dynamic, object> expression) => Parse(expression, out var _, out var _);

    /// <summary>
    /// Parses the given dynamic lambda expression returning the multipart name it resolves
    /// into, where these parts are joined by dots. If any part is null or empty (including
    /// those whose name resolve into the name of the dynamic argument), then it is parsed as
    /// an empty literal. If all parts are null or empty, then an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="darg"></param>
    /// <returns></returns>
    public static string? Parse(
        Func<dynamic, object> expression, out DynamicNodeArgument darg)
        => Parse(expression, out var _, out darg);

    /// <summary>
    /// Parses the given dynamic lambda expression returning the multipart name it resolves
    /// into, where these parts are joined by dots, and returned in the out argument. If any
    /// part is null or empty (including those whose name resolve into the name of the dynamic
    /// argument), then it is parsed as an empty literal. If all parts are null or empty, then
    /// an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string? Parse(
        Func<dynamic, object> expression, out string?[] parts)
        => Parse(expression, out parts, out var _);

    /// <summary>
    /// Parses the given dynamic lambda expression returning the multipart name it resolves
    /// into, where these parts are joined by dots, and returned in the out argument. If any
    /// part is null or empty (including those whose name resolve into the name of the dynamic
    /// argument), then it is parsed as an empty literal. If all parts are null or empty, then
    /// an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="darg"></param>
    /// <returns></returns>
    public static string? Parse(
        Func<dynamic, object> expression, out string?[] parts, out DynamicNodeArgument darg)
    {
        expression = expression.ThrowIfNull();

        var parser = DynamicParser.Parse(expression);
        var list = new List<string?>();

        var xarg = parser.Argument;
        darg = xarg;

        OnParse(parser.Result);
        parts = list.ToArray();

        return parts.All(x => x == null) ? null : string.Join('.', parts);

        // Recursive entry point...
        void OnParse(DynamicNode node)
        {
            switch (node)
            {
                case DynamicNodeArgument item: return;
                case DynamicNodeMember item: OnParseMember(item); return;
                case DynamicNodeValued item: OnParseValue(item); return;
            }

            throw new ArgumentException(
                "Expression carries a not supported node.")
                .WithData(node)
                .WithData(parser, nameof(expression));
        }

        // Parses a get member node...
        void OnParseMember(DynamicNodeMember node)
        {
            OnParse(node.DynamicHost);

            var name = xarg.DynamicName == node.DynamicName
                ? null
                : node.DynamicName;

            list.Add(name);
        }

        // Parses a constant node...
        void OnParseValue(DynamicNodeValued node)
        {
            switch (node.DynamicValue)
            {
                case string str:
                    var names = str.Split('.');
                    for (int i = 0; i < names.Length; i++)
                    {
                        var name = names[i].NullWhenEmpty(trim: true);
                        if (name == xarg.DynamicName) name = null;
                        list.Add(name);
                    }
                    return;

                case null:
                    list.Add(null);
                    return;
            }

            throw new ArgumentException(
                "Expression carries a not supported constant node.")
                .WithData(node)
                .WithData(parser, nameof(expression));
        }
    }
}
