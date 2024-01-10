#pragma warning disable IDE0200

namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions and nodes and returning a token
/// that represents their contents as database tokens.
/// </summary>
/// <param name="engine"></param>
public class TokenParser(IEngine engine)
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; } = engine.ThrowWhenNull();

    /// <summary>
    /// Parses the given dynamic lambda expression returning a token that represents its contents.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public Token Parse(Func<dynamic, object> expression)
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
    public Token Parse(LambdaNode node)
    {
        node.ThrowWhenNull();

        return node switch
        {
            LambdaNodeArgument item => ParseArgument(item),
            LambdaNodeBinary item => ParseBinary(item),
            LambdaNodeConvert item => ParseConvert(item),
            LambdaNodeIndexed item => ParseIndexed(item),
            LambdaNodeInvoke item => ParseInvoke(item),
            LambdaNodeMember item => ParseMember(item),
            LambdaNodeMethod item => ParseMethod(item),
            LambdaNodeSetter item => ParseSetter(item),
            LambdaNodeUnary item => ParseUnary(item),
            LambdaNodeValue item => ParseValue(item),
            LambdaNodeCoalesce item => ParseCoalesce(item),
            LambdaNodeTernary item => ParseTernary(item),

            _ => throw new UnExpectedException("Unknown node.").WithData(node)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static TokenArgument ParseArgument(LambdaNodeArgument node)
    {
        return new TokenArgument(node.LambdaName);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenBinary ParseBinary(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new TokenBinary(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenConvertToType ParseConvert(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new TokenConvertToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenIndexed ParseIndexed(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new TokenIndexed(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenInvoke ParseInvoke(LambdaNodeInvoke node)
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
    TokenIdentifier ParseMember(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        return new TokenIdentifier(host, new Code.IdentifierPart(Engine, name));
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    Token ParseMethod(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, false);

        if (name == null) // Translate to invoke...
        {
            if (node.LambdaTypeArguments.Count != 0) throw new ArgumentException(
                "Invoke tokens do not support generic type arguments.")
                .WithData(node);

            var temp = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return ParseInvoke(temp);
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
    private TokenSetter ParseSetter(LambdaNodeSetter node)
    {
        var target = Parse(node.LambdaTarget);
        var value = Parse(node.LambdaValue);

        return new TokenSetter(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenUnary ParseUnary(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new TokenUnary(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    Token ParseValue(LambdaNodeValue node)
    {
        return node.LambdaValue switch
        {
            Token item => item,
            LambdaNode item => Parse(item),

            Delegate item => throw new ArgumentException(
                $"Cannot use a delegate as the value of a '{nameof(TokenValue)}' token.")
                .WithData(item),

            _ => new TokenValue(node.LambdaValue)
        };
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenCoalesce ParseCoalesce(LambdaNodeCoalesce node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new TokenCoalesce(left, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    TokenTernary ParseTernary(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);

        return new TokenTernary(left, middle, right);
    }
}