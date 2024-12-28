using System.Runtime.InteropServices.Marshalling;

namespace Yotei.Tools;

// ========================================================
public partial class LambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression that resolves into a dot-separated member
    /// name, and returns that name. If all parts are null or empty, then an empty string
    /// is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression)
    {
        return ParseName(expression, out _, out _);
    }

    /// <summary>
    /// Parses the given dynamic lambda expression that resolves into a dot-separated member
    /// name, and returns that name, along with its individual parts. If all parts are null or
    /// empty, then an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression, out string[] parts)
    {
        return ParseName(expression, out parts, out _);
    }

    /// <summary>
    /// Parses the given dynamic lambda expression that resolves into a dot-separated member
    /// name, and returns that name, along with the name of the dynamic argument used in the
    /// expression. If all parts are null or empty, then an empty string is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(Func<dynamic, object> expression, out LambdaNodeArgument arg)
    {
        return ParseName(expression, out _, out arg);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given dynamic lambda expression that resolves into a dot-separated member
    /// name, and returns that name, along with its individual parts and the name of the dynamic
    /// argument used in the expression. If all parts are null or empty, then an empty string
    /// is returned.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string ParseName(
        Func<dynamic, object> expression, out string[] parts, out LambdaNodeArgument arg)
    {
        expression.ThrowWhenNull();

        var parser = Parse(expression);
        if (parser.DynamicArguments.Length != 1) throw new ArgumentException(
            "Dynamic lambda expression needs one and only one dynamic argument.")
            .WithData(parser, nameof(expression));

        var xarg = parser.DynamicArguments[0]; // Using var 'xarg' because it is used later...
        arg = xarg;

        var list = new List<string>();
        OnParseName(parser.Result);
        parts = list.ToArray();

        for (int i = 0; i < parts.Length; i++) if (parts[i] is null) parts[i] = string.Empty;
        return parts.All(x => x.Length == 0) ? string.Empty : string.Join('.', parts);

        /// <summary>
        /// Recursive entry point.
        /// </summary>
        void OnParseName(LambdaNode node)
        {
            switch (node)
            {
                case LambdaNodeArgument: return;
                case LambdaNodeMember item: OnParseMember(item); return;
                case LambdaNodeValue item: OnParseConstant(item); return;

                default: throw new ArgumentException(
                    "Expression carries a not supported node.")
                    .WithData(node)
                    .WithData(parser, nameof(expression));
            }
        }

        /// <summary>
        /// Parses a <see cref="LambdaNodeMember"/> node.
        /// </summary>
        void OnParseMember(LambdaNodeMember node)
        {
            OnParseName(node.LambdaHost);

            var name = node.LambdaName == xarg.LambdaName
                ? null
                : node.LambdaName;

            list.Add(name!);
        }

        /// <summary>
        /// Parses a <see cref="LambdaNodeValue"/> node.
        /// </summary>
        void OnParseConstant(LambdaNodeValue node)
        {
            if (node.LambdaValue is null)
            {
                list.Add(null!);
            }
            else if (node.LambdaValue is string str)
            {
                var names = str.Split('.');
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i].NullWhenEmpty(trim: true);
                    if (name != null &&
                        name == xarg.LambdaName) name = null;

                    list.Add(name!);
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