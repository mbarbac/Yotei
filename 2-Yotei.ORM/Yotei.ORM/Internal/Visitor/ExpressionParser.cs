#pragma warning disable IDE0200

namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions and nodes and returning a token
/// that represents their contents as database tokens.
/// </summary>
public static class ExpressionParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression returning a token that represents its contents.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static Token Parse(Func<dynamic, object> expression)
    {
        var parser = LambdaParser.Parse(expression);
        var token = Parse(parser.Result);
        return token;
    }

    /// <summary>
    /// Parses the given dynamic lambda node returning a token that represents its contents.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="darg"></param>
    /// <returns></returns>
    public static Token Parse(LambdaNode node)
    {
        node.ThrowWhenNull();

        return node switch
        {
            LambdaNodeArgument item => Parse(item),
            LambdaNodeBinary item => Parse(item),
            LambdaNodeConvert item => Parse(item),
            LambdaNodeIndexed item => Parse(item),
            LambdaNodeInvoke item => Parse(item),
            LambdaNodeMember item => Parse(item),
            LambdaNodeMethod item => Parse(item),
            LambdaNodeSetter item => Parse(item),
            LambdaNodeUnary item => Parse(item),
            LambdaNodeValue item => Parse(item),
            LambdaNodeCoalesce item => Parse(item),
            LambdaNodeTernary item => Parse(item),

            _ => throw new UnExpectedException("Unknown node.").WithData(node)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenArgument Parse(LambdaNodeArgument node)
    {
        return new TokenArgument(node.LambdaName);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenBinary Parse(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new TokenBinary(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenConvertToType Parse(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new TokenConvertToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenIndexed Parse(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new TokenIndexed(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenInvoke Parse(LambdaNodeInvoke node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();

        // Single string argument is considered a literal...
        if (items.Count == 1 &&
            items[0] is TokenValue value &&
            value.Value is string str)
            items[0] = new TokenLiteral(str);

        return new TokenInvoke(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenIdentifier Parse(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, false);

        return new TokenIdentifier(host, new Code.IdentifierPart(name));
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static Token Parse(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, false);

        if (name == null) // Translate to invoke...
        {
            if (node.LambdaTypeArguments.Count != 0) throw new ArgumentException(
                "Invoke tokens do not support generic type arguments.")
                .WithData(node);

            var temp = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return Parse(temp);
        }
        else // Other cases...
        {
            // x => x.Convert(type, target)...
            if (node.LambdaHost is LambdaNodeArgument &&
                node.LambdaArguments.Count == 2 &&
                node.LambdaArguments[0] is LambdaNodeValue one &&
                string.Compare(node.LambdaName, "Convert", ignoreCase: true) == 0)
            {
                var target = Parse(node.LambdaArguments[1]);
                switch (one.LambdaValue)
                {
                    case Type oneType: return new TokenConvertToType(oneType, target);
                    case string oneSpec: return new TokenConvertToSpecification(oneSpec, target);
                }
            }

            // x => x.Coalesce(left, right)
            if (node.LambdaHost is LambdaNodeArgument &&
                node.LambdaArguments.Count == 2 &&
                string.Compare(node.LambdaName, "Coalesce", ignoreCase: true) == 0)
            {
                var left = Parse(node.LambdaArguments[0]);
                var right = Parse(node.LambdaArguments[1]);

                return new TokenCoalesce(left, right);
            }

            // x => x.Ternary(left, middle, right)
            if (node.LambdaHost is LambdaNodeArgument &&
                node.LambdaArguments.Count == 3 &&
                string.Compare(node.LambdaName, "Ternary", ignoreCase: true) == 0)
            {
                var left = Parse(node.LambdaArguments[0]);
                var middle = Parse(node.LambdaArguments[1]);
                var right = Parse(node.LambdaArguments[2]);

                return new TokenTernary(left, middle, right);
            }

            // Standard case...
            var host = Parse(node.LambdaHost);
            var items = node.LambdaArguments.Select(x => Parse(x));

            return node.LambdaTypeArguments.Count == 0
                ? new TokenMethod(host, name, items)
                : new TokenMethod(host, name, node.LambdaTypeArguments, items);
        }
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenSetter Parse(LambdaNodeSetter node)
    {
        var target = Parse(node.LambdaTarget);
        var value = Parse(node.LambdaValue);

        return new TokenSetter(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenUnary Parse(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new TokenUnary(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static Token Parse(LambdaNodeValue node)
    {
        return node.LambdaValue switch
        {
            Token item => item,
            LambdaNode item => Parse(item),
            ICommand item => new TokenCommand(item),

            Delegate item => throw new ArgumentException(
                $"Cannot use a delegate as the value of a '{nameof(TokenValue)}' token.")
                .WithData(item),

            _ => new TokenValue(node.LambdaValue)
        };
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenCoalesce Parse(LambdaNodeCoalesce node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new TokenCoalesce(left, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenTernary Parse(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);

        return new TokenTernary(left, middle, right);
    }
}