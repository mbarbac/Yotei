namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last db-alike
/// token in a chain that represents the dynamic operations in that expression.
/// </summary>
public class DbLambdaParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public DbLambdaParser(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given dynamic lambda expression returning the last db-alike node in the chain
    /// that represents the dynamic operations in that expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public DbToken Parse(Func<dynamic, object> expression)
    {
        var parser = LambdaParser.Parse(expression);
        var token = Parse(parser.Result);
        return token;
    }

    /// <summary>
    /// Parses the given dynamic lambda node returning the last db-alike node in the chain that
    /// represents the dynamic node's one.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public DbToken Parse(LambdaNode node)
    {
        node.ThrowWhenNull();

        return node switch
        {
            LambdaNodeArgument item => ParseArgument(item),
            LambdaNodeBinary item => ParseBinary(item),
            LambdaNodeCoalesce item => ParseCoalesce(item),
            LambdaNodeConvert item => ParseConvert(item),
            LambdaNodeIndexed item => ParseIndexed(item),
            LambdaNodeInvoke item => ParseInvoke(item),
            LambdaNodeMember item => ParseMember(item),
            LambdaNodeMethod item => ParseMethod(item),
            LambdaNodeSetter item => ParseSetter(item),
            LambdaNodeTernary item => ParseTernary(item),
            LambdaNodeUnary item => ParseUnary(item),
            LambdaNodeValue item => ParseValue(item),

            _ => throw new ArgumentException("Unknown node.").WithData(node)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static DbTokenArgument ParseArgument(LambdaNodeArgument node) => new(node.LambdaName);

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenBinary ParseBinary(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenCoalesce ParseCoalesce(LambdaNodeCoalesce node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new(left, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenConvert.ToType ParseConvert(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenIndexed ParseIndexed(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbToken ParseInvoke(LambdaNodeInvoke node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();

        if (items.Count == 1)
        {
            // Single-command argument...
            if (items[0] is DbTokenCommand command) return command;

            // Intercepting stand-alone strings, but only modifying the token...
            if (items[0] is DbTokenValue value &&
                value.Value is string str) items[0] = new DbTokenLiteral(str);
        }

        // Standard case...
        return new DbTokenInvoke(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenIdentifier ParseMember(LambdaNodeMember node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenArgument ParseMethod(LambdaNodeMethod node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenArgument ParseSetter(LambdaNodeSetter node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenArgument ParseTernary(LambdaNodeTernary node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenArgument ParseUnary(LambdaNodeUnary node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    DbTokenArgument ParseValue(LambdaNodeValue node)
    {
        throw null;
    }
}