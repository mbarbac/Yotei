#pragma warning disable CA1822, CA1859

using System.Diagnostics.Contracts;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, returning the last database
/// alike token in the chain that contains the dynamic operations in that expression.
/// </summary>
/// <Notes>
/// This class provides a fast parsing mechanism from dynamic lambda expressions to database
/// tokens. Later, visitors can translate those token chains into the appropriate command info
/// instances for their database engine types.
/// </Notes>
public record DbLambdaParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public DbLambdaParser(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The engine this parser is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Determines if conversions from single string values of invoke nodes to literal string ones
    /// shall be prevented, or not.
    /// </summary>
    public bool PreventSingleStringValueToLiteral { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given dynamic lambda expression representing a chain of dynamic operations, and
    /// returns the last database-alike token that contains the dynamic operations in that chain.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public IDbToken Parse(Func<dynamic, object?> expression)
    {
        var parser = LambdaParser.Parse(expression);
        var node = parser.Result;
        var token = Parse(node);
        return token;
    }

    /// <summary>
    /// Parses the given dynamic lambda node representing a chain of dynamic operations, and
    /// returns the last database-alike token that contains the dynamic operations in that chain.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public IDbToken Parse(LambdaNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var token = node switch
        {
            LambdaNodeArgument item => ParseNode(item),
            LambdaNodeBinary item => ParseNode(item),
            LambdaNodeCoalesce item => ParseNode(item),
            LambdaNodeConvert item => ParseNode(item),
            LambdaNodeIndexed item => ParseNode(item),
            LambdaNodeInvoke item => ParseNode(item),
            LambdaNodeMember item => ParseNode(item),
            LambdaNodeMethod item => ParseNode(item),
            LambdaNodeSetter item => ParseNode(item),
            LambdaNodeTernary item => ParseNode(item),
            LambdaNodeUnary item => ParseNode(item),
            LambdaNodeValue item => ParseNode(item),

            _ => throw new ArgumentException("Unknown node.").WithData(node)
        };

        //if (token is DbTokenChain chain) token = chain.Reduce(); // TODO...
        return token;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeArgument node) => new DbTokenArgument(node.LambdaName);

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);
        return new DbTokenBinary(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeCoalesce node)
    {
        // Shortcut when we can intercept a null-alike left argument...
        if (node.LambdaLeft is LambdaNodeValue value &&
            value.LambdaValue is null)
        {
            return Parse(node.LambdaRight);
        }
        else // Standard case...
        {
            var left = Parse(node.LambdaLeft);
            var right = Parse(node.LambdaRight);
            return new DbTokenCoalesce(left, right);
        }
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));
        return new DbTokenIndexed(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeInvoke node)
    {
        // Special cases are always firts-level ones...
        if (node.LambdaHost is LambdaNodeArgument)
        {
            // Single argument...
            if (node.LambdaArguments.Length == 1)
            {
                if (node.LambdaArguments[0] is LambdaNodeValue value)
                {
                    // Convert to literal...
                    if (!PreventSingleStringValueToLiteral &&
                        value.LambdaValue is string str)
                        return new DbTokenLiteral(str);

                    // Command-alike...
                    if (value.LambdaValue is ICommandInfo info) return new DbTokenCommandInfo(info);
                    if (value.LambdaValue is ICommand command)
                        return new DbTokenCommandInfo(command.GetCommandInfo(iterable: false));
                }
            }
        }

        // Default case...
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();
        return new DbTokenInvoke(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.IgnoreCase);

        var identifier = name is null ? new Identifier(Engine) : new Identifier(Engine, name);
        return new DbTokenIdentifier(host, identifier);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.IgnoreCase);

        // Intercepts 'x => x(...)' && 'x => x.Any.x(...)':
        if (name is null)
        {
            if (node.LambdaTypeArguments.Length != 0) throw new ArgumentException(
                "Cannot use type arguments with invoke-alike method tokens.")
                .WithData(node);

            var invoke = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return ParseNode(invoke);
        }

        // Intercepts 'Coalesce' virtual method:
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Length == 2 &&
            string.Compare(name, "Coalesce", Engine.IgnoreCase) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var right = Parse(node.LambdaArguments[1]);
            return new DbTokenCoalesce(left, right);
        }

        // Intercepts 'Ternary' virtual method:
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Length == 3 &&
            string.Compare(name, "Ternary", Engine.IgnoreCase) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var middle = Parse(node.LambdaArguments[1]);
            var right = Parse(node.LambdaArguments[2]);
            return new DbTokenTernary(left, middle, right);
        }

        // Intercepts 'Convert' and 'Cast' virtual methods:
        if (node.LambdaHost is LambdaNodeArgument && (
            string.Compare(name, "Convert", Engine.IgnoreCase) == 0 ||
            string.Compare(name, "Cast", Engine.IgnoreCase) == 0))
        {
            // x => x.Convert<T>(target):
            if (node.LambdaTypeArguments.Length == 1 &&
                node.LambdaArguments.Length == 1)
            {
                var type = node.LambdaTypeArguments[0];
                var target = Parse(node.LambdaArguments[0]);
                return new DbTokenConvert.ToType(type, target);
            }

            // x => x.Convert(spec, target):
            if (node.LambdaTypeArguments.Length == 0 &&
                node.LambdaArguments.Length == 2 &&
                node.LambdaArguments[0] is LambdaNodeValue value)
            {
                var target = Parse(node.LambdaArguments[1]);
                switch (value.LambdaValue)
                {
                    case Type item: return new DbTokenConvert.ToType(item, target);
                    case string item: return new DbTokenConvert.ToSpec(item, target);
                }
            }
        }

        // Standard case...
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x));
        return new DbTokenMethod(host, name, Engine.IgnoreCase, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeSetter node)
    {
        var target = Parse(node.LambdaTarget);
        var value = Parse(node.LambdaValue);
        return new DbTokenSetter(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);
        return new DbTokenTernary(left, middle, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenUnary(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken ParseNode(LambdaNodeValue node) => node.LambdaValue switch
    {
        IDbToken item => item,
        LambdaNode item => Parse(item),
        ICommandInfo item => new DbTokenCommandInfo(item),
        ICommand item => new DbTokenCommandInfo(item.GetCommandInfo(iterable: false)),

        Delegate => throw new ArgumentException(
            "Delegates cannot be used as the value of a lambda node.")
            .WithData(node),

        _ => new DbTokenValue(node.LambdaValue)
    };
}