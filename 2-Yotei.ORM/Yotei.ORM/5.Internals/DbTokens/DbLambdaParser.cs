#pragma warning disable IDE0079
#pragma warning disable CA1822
#pragma warning disable CA1859

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last database-alike
/// token in the chain that contains the dynamic operations of that expression.
/// </summary>
/// The objective of this class is to provide a fast parsing of dynamic lambda expressions into
/// database alike-tokens. Later, visitors can translate those token chains into the command-info
/// objects that contains the appropriate text and arguments for the underlying database.
public record DbLambdaParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public DbLambdaParser(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The engine used by this instance.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// If set, prevents conversion of invoke nodes whose unique argument is a string to literal
    /// nodes.
    /// </summary>
    public bool PreventValueStringToLiteral { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations of that expression.
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
    /// Parses the given dynamic lambda node that represents a chain of dynamic operations, and
    /// returns the last database-alike token in the chain that contains the dynamic operations
    /// of that lambda token.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public IDbToken Parse(LambdaNode node)
    {
        Engine.ThrowWhenNull();
        node.ThrowWhenNull();

        var temp = node switch
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

        if (temp is DbTokenChain chain) temp = chain.Reduce();
        return temp;
    }

    // ====================================================

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeArgument node)
    {
        return new DbTokenArgument(node.LambdaName);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeBinary node)
    {
        var left = Parse(node.LambdaLeft);
        var right = Parse(node.LambdaRight);

        return new DbTokenBinary(left, node.LambdaOperation, right);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeCoalesce node)
    {
        // Special case when we can intercept a null-alike left-argument...
        if (node.LambdaLeft is LambdaNodeValue value && value.LambdaValue is null)
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

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new DbTokenIndexed(host, items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeInvoke node)
    {
        // Intercepting special cases...
        if (node.LambdaHost is LambdaNodeArgument)
        {
            // Single-argument...
            if (node.LambdaArguments.Count == 1)
            {
                if (node.LambdaArguments[0] is LambdaNodeValue value)
                {
                    // Convert to literal...
                    if (!PreventValueStringToLiteral &&
                        value.LambdaValue is string str)
                        return new DbTokenLiteral(str);

                    // Command-alike...
                    if (value.LambdaValue is ICommand command) return new DbTokenCommand(command);
                    if (value.LambdaValue is ICommandInfo info) return new DbTokenCommandInfo(info);
                }
            }

            // Two-arguments...
            if (node.LambdaArguments.Count == 2)
            {
                if (node.LambdaArguments[0] is LambdaNodeValue value &&
                    node.LambdaArguments[1] is LambdaNodeValue temp &&
                    value.LambdaValue is ICommand command &&
                    temp.LambdaValue is bool clone)
                    return new DbTokenCommand(command, clone);
            }
        }

        // Default-case...
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();
        return new DbTokenInvoke(host, items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        var id = name == null ? new IdentifierUnit(Engine) : new IdentifierUnit(Engine, name);
        return new DbTokenIdentifier(host, id);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// <br/> Translates 'x => x(...)' and 'x => x.Any.x(...)' to invoke (case sensitive!).
    /// <br/> Intercepts 'x => x.Coalesce(...)' virtual method.
    /// <br/> Intercepts 'x => x.Ternary(...)' virtual method.
    /// <br/> Intercepts 'x => x.Convert(...)' and 'x => x.Cast(...)' virtual methods.
    /// </summary>
    IDbToken ParseNode(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, caseSensitive: true);

        // 'x => x(...)' and 'x => x.Any.x(...)' to invoke...
        if (name == null)
        {
            if (node.LambdaGenericArguments.Count != 0) throw new ArgumentException(
                "Cannot use type arguments with invoke tokens.")
                .WithData(node);

            var invoke = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return ParseNode(invoke);
        }

        // Intercepts 'Coalesce' virtual method...
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 2 &&
            string.Compare(name, "Coalesce", !Engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var right = Parse(node.LambdaArguments[1]);

            return new DbTokenCoalesce(left, right);
        }

        // Intercepts 'Ternary' virtual method...
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 3 &&
            string.Compare(name, "Ternary", !Engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(node.LambdaArguments[0]);
            var middle = Parse(node.LambdaArguments[1]);
            var right = Parse(node.LambdaArguments[2]);

            return new DbTokenTernary(left, middle, right);
        }

        // Intercepts 'Convert' and 'Cast' virtual methods...
        if (node.LambdaHost is LambdaNodeArgument && (
            string.Compare(name, "Convert", !Engine.CaseSensitiveNames) == 0 ||
            string.Compare(name, "Cast", !Engine.CaseSensitiveNames) == 0))
        {
            // x => x.Convert<T>(target)...
            if (node.LambdaGenericArguments.Count == 1 &&
                node.LambdaArguments.Count == 1)
            {
                var type = node.LambdaGenericArguments[0];
                var target = Parse(node.LambdaArguments[0]);

                return new DbTokenConvert.ToType(type, target);
            }

            // x => x.Convert(type|spec, target)...
            if (node.LambdaArguments.Count == 2 &&
                node.LambdaArguments[0] is LambdaNodeValue value)
            {
                var target = Parse(node.LambdaArguments[1]);
                switch (value.LambdaValue)
                {
                    case Type vtype: return new DbTokenConvert.ToType(vtype, target);
                    case string vspec: return new DbTokenConvert.ToSpec(vspec, target);
                }
            }
        }

        // Standard case...
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x));

        return new DbTokenMethod(host, name, node.LambdaGenericArguments, items);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeSetter node)
    {
        var target = Parse(node.LambdaTarget);
        var value = Parse(node.LambdaValue);

        return new DbTokenSetter(target, value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);

        return new DbTokenTernary(left, middle, right);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenUnary(node.LambdaOperation, target);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    IDbToken ParseNode(LambdaNodeValue node)
    {
        return node.LambdaValue switch
        {
            IDbToken item => item,
            LambdaNode item => Parse(item),
            ICommand item => new DbTokenCommand(item),
            ICommandInfo item => new DbTokenCommandInfo(item),

            Delegate => throw new ArgumentException(
                "Cannot use delegate as the value of lambda nodes.")
                .WithData(node),

            _ => new DbTokenValue(node.LambdaValue),
        };
    }
}