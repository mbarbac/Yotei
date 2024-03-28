#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0200 // Lambda expression can be removed

namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions and nodes and returning a token
/// that represents their contents as database tokens.
/// </summary>
public class ExpressionParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public ExpressionParser(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Parses the given dynamic lambda expression returning a token that represents its logic
    /// and contents.
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
    /// Parses the given dynamic lambda node returning a token that represents its logic and
    /// contents.
    /// </summary>
    /// <param name="node"></param>
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
    /// <param name="node"></param>
    /// <returns></returns>
    TokenArgument ParseArgument(LambdaNodeArgument node) => new(node.LambdaName);

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenBinary ParseBinary(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    [SuppressMessage("","CA1859")]
    TokenConvert ParseConvert(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new TokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenIndexed ParseIndexed(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// <br/> This method may also:
    /// <br/> - Return a TokenCommand instance, if the sole argument is such,
    /// <br/> - Translate the sole string argument into a TokenLiteral one.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    Token ParseInvoke(LambdaNodeInvoke node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();

        // Single command argument...
        if (items.Count == 1 && items[0] is TokenCommand command) return command;

        // Intercepting stand-alone strings...
        if (items.Count == 1 && items[0] is TokenValue value && value.Value is string str)
            items[0] = new TokenLiteral(str);

        // Standard case...
        return new TokenInvoke(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenIdentifier ParseMember(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        return new(host, new IdentifierPart(Engine, name));
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    Token ParseMethod(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        // Translate to invoke: x => x(...), x => x.Any.x(...)
        if (name == null)
        {
            if (node.LambdaTypeArguments.Count != 0) throw new ArgumentException(
                "Invoke tokens do not support generic type arguments.")
                .WithData(node);

            var invoke = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return ParseInvoke(invoke);
        }

        // Translate to coalesce: x => x.Coalesce(left, right)
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 2 &&
            string.Compare(name, "Coalesce", !Engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var right = Parse(node.LambdaArguments[1]);

            return new TokenCoalesce(left, right);
        }

        // Transale to ternary: x => x.Ternary(left, middle, right)
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 3 &&
            string.Compare(name, "Ternary", !Engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var middle = Parse(node.LambdaArguments[1]);
            var right = Parse(node.LambdaArguments[2]);

            return new TokenTernary(left, middle, right);
        }

        // Transalate to convert:
        if (node.LambdaHost is LambdaNodeArgument && (
            string.Compare(name, "Convert", !Engine.CaseSensitiveNames) == 0 ||
            string.Compare(name, "Cast", !Engine.CaseSensitiveNames) == 0))
        {
            // x => x.Convert<T>(target...)...
            if (node.LambdaTypeArguments.Count == 1 &&
                node.LambdaArguments.Count == 1)
            {
                var type = node.LambdaTypeArguments[0];
                var target = Parse(node.LambdaArguments[0]);
                return new TokenConvert.ToType(type, target);
            }

            // x => x.Convert(type|spec, target)...
            if (node.LambdaArguments.Count == 2 &&
                node.LambdaArguments[0] is LambdaNodeValue value)
            {
                var target = Parse(node.LambdaArguments[1]);
                switch (value.LambdaValue)
                {
                    case Type vtype: return new TokenConvert.ToType(vtype, target);
                    case string vspec: return new TokenConvert.ToSpec(vspec, target);
                }
            }
        }

        // Standar case...
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x));

        return node.LambdaTypeArguments.Count == 0
            ? new TokenMethod(host, name, items)
            : new TokenMethod(host, name, node.LambdaTypeArguments, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenSetter ParseSetter(LambdaNodeSetter node)
    {
        var target = Parse(node.LambdaTarget);
        var value = Parse(node.LambdaValue);

        return new(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenUnary ParseUnary(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    Token ParseValue(LambdaNodeValue node) => node.LambdaValue switch
    {
        Token item => item,
        LambdaNode item => Parse(item),
        ICommand item => new TokenCommand(item.GetCommandInfo(iterable: false)),

        Delegate item => throw new ArgumentException(
            "Cannot use delegates as the values of tokens.")
            .WithData(item),

        _ => new TokenValue(node.LambdaValue)
    };

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenCoalesce ParseCoalesce(LambdaNodeCoalesce node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new(left, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    TokenTernary ParseTernary(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);

        return new(left, middle, right);
    }
}