namespace Yotei.Tools;

// ========================================================
public partial class LambdaParser
{
    /// <summary>
    /// Parses the guven dynamic lambda expression returning a string with the dot-separated
    /// parts that correspond with the chain of names that expression resolved into. Parts whose
    /// value is the same as the name of the dynamic argument are parsed as null literals. If
    /// all parts are null or empty, then an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression)
    {
        return ParseName(expression, out _, out _);
    }

    /// <summary>
    /// <inheritdoc cref="ParseName(Func{dynamic, object})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression, out string?[] parts)
    {
        return ParseName(expression, out parts, out _);
    }

    /// <summary>
    /// <inheritdoc cref="ParseName(Func{dynamic, object})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression, out LambdaNodeArgument arg)
    {
        return ParseName(expression, out _, out arg);
    }

    /// <summary>
    /// <inheritdoc cref="ParseName(Func{dynamic, object})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression, out string?[] parts, out LambdaNodeArgument arg)

    {
        expression = expression.ThrowWhenNull();

        var parser = Parse(expression);
        var xarg = parser.Argument;
        arg = xarg;

        var list = new List<string?>();
        OnParse(parser.Result);
        parts = list.ToArray();

        return parts.All(x => x == null) ? string.Empty : string.Join('.', parts);

        /// <summary>
        /// Recursive entry point.
        /// </summary>
        void OnParse(LambdaNode node)
        {
            switch (node)
            {
                case LambdaNodeArgument: return;
                case LambdaNodeMember item: OnParseMember(item); return;
                case LambdaNodeConstant item: OnParseConstant(item); return;

                default:
                    throw new ArgumentException(
                        "Expression carries a not supported node.")
                        .WithData(node)
                        .WithData(parser);
            }
        }

        /// <summary>
        /// Parses a GetMember node.
        /// </summary>
        void OnParseMember(LambdaNodeMember node)
        {
            OnParse(node.LambdaHost);

            var name = xarg.LambdaName == node.LambdaName
                ? null
                : node.LambdaName;

            list.Add(name);
        }

        /// <summary>
        /// Parses a constant node.
        /// </summary>
        void OnParseConstant(LambdaNodeConstant node)
        {
            if (node.LambdaValue is null)
            {
                list.Add(null);
            }
            else if (node.LambdaValue is string str)
            {
                var names = str.Split('.');
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i].NullWhenEmpty();
                    if (name != null &&
                        name == xarg.LambdaName) name = null;

                    list.Add(name);
                }
            }
            else
            {
                throw new ArgumentException(
                    "Expression carries a not supported constant node.")
                    .WithData(node)
                    .WithData(parser, nameof(expression));
            }
        }
    }
}