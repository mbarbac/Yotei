#pragma warning disable IDE0079
#pragma warning disable IDE0060
#pragma warning disable CA1859

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last database-alike
/// token in the chain that contains the dynamic operations in that expression.
/// <br/>- Standard syntax: 'x => x...'
/// <br/>- Alternate syntax: 'x => "..."', where the string contents are taken as literal ones.
/// </summary>
/// <remarks>The objective of this class is to provide a fast first-pass parsing of dynamic lambda
/// nodes translating them into database-command tokens. Later, specific visitor instances can
/// translate those tokens into the appropriate command-info object that contains the right SQL
/// text for the underlying database engine, and the captured SQL arguments
/// </remarks>
public static class DbLambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations in that expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="engine"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static IDbToken Parse<T>(IEngine engine, Func<dynamic, T> expression)
    {
        var parser = LambdaParser.Parse(expression);
        var node = parser.Result;

        var method = expression.GetMethodInfo();

        // Special case for 'x => "string"' syntax...
        if (method.ReturnType == typeof(string) &&
            node is LambdaNodeValue value && value.LambdaValue is string str)
        {
            return new DbTokenLiteral(str);
        }

        // Standard 'x => ...'  syntax...
        {
            var token = Parse(engine, node);
            return token;
        }
    }

    /// <summary>
    /// Parses the given lambda node and returns the last database-alike token in the chain
    /// that contains the dynamic operations in that node.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IDbToken Parse(IEngine engine, LambdaNode node)
    {
        node.ThrowWhenNull();

        var temp = node switch
        {
            LambdaNodeArgument item => Parse(engine, item),
            LambdaNodeBinary item => Parse(engine, item),
            LambdaNodeCoalesce item => Parse(engine, item),
            LambdaNodeConvert item => Parse(engine, item),
            LambdaNodeIndexed item => Parse(engine, item),
            LambdaNodeInvoke item => Parse(engine, item),
            LambdaNodeMember item => Parse(engine, item),
            LambdaNodeMethod item => Parse(engine, item),
            LambdaNodeSetter item => Parse(engine, item),
            LambdaNodeTernary item => Parse(engine, item),
            LambdaNodeUnary item => Parse(engine, item),
            LambdaNodeValue item => Parse(engine, item),

            _ => throw new ArgumentException("Unknown node.").WithData(node)
        };

        if (temp is DbTokenChain chain) temp = chain.Reduce();
        return temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeArgument node)
    {
        return new DbTokenArgument(node.LambdaName);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeBinary node)
    {
        var left = Parse(engine, node.LambdaLeft);
        var right = Parse(engine, node.LambdaRight);

        return new DbTokenBinary(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeCoalesce node)
    {
        // Special case when we can intercept a null-alike left-argument...
        if (node.LambdaLeft is LambdaNodeValue value && value.LambdaValue is null)
        {
            return Parse(engine, node.LambdaRight);
        }
        else // Standard case...
        {
            var left = Parse(engine, node.LambdaLeft);
            var right = Parse(engine, node.LambdaRight);

            return new DbTokenCoalesce(left, right);
        }
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeConvert node)
    {
        var target = Parse(engine, node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeIndexed node)
    {
        var host = Parse(engine, node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(engine, x));

        return new DbTokenIndexed(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// <br/>- Single command-alike arguments '(cmd)' are translated into a command instances.
    /// <br/>- Unique '(str)' string arguments are translated into literal **arguments**...
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeInvoke node)
    {
        var host = Parse(engine, node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(engine, x)).ToList();

        if (items.Count == 1) // Intercepting special cases...
        {
            // TODO: invoke conversion to command looses track of argument.
            // In syntaxes as '(SELECT..).As()' the first token is rendered as a plain command
            // one, losing track of the dynamic argument: this cause problems when methods as
            // 'RemoveFirst()' are used, because they expect the first element in the tree to
            // be a hosted one with a dynamic argument as a host. At the end of the day it looks
            // like an over-engineered interception...

            // Command-alike tokens...
            // if (items[0] is DbTokenCommand command) return command;

            // Literal-alike tokens...
            if (items[0] is DbTokenValue value && value.Value is string str)
                items[0] = new DbTokenLiteral(str);
        }

        // Standard cases...
        return new DbTokenInvoke(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeMember node)
    {
        var host = Parse(engine, node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, engine.CaseSensitiveNames);

        var id = name == null ? new IdentifierPart(engine) : new IdentifierPart(engine, name);
        return new DbTokenIdentifier(host, id);
    }

    /// <summary>
    /// Parses the given node.
    /// <br/> Translates 'x => x(...)' and 'x => x.Any.x(...)' to invoke (case sensitive!).
    /// <br/> Intercepts 'x => x.Coalesce(...)' virtual method.
    /// <br/> Intercepts 'x => x.Ternary(...)' virtual method.
    /// <br/> Intercepts 'x => x.Convert(...)' and 'x => x.Cast(...)' virtual methods.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeMethod node)
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
            return Parse(engine, invoke);
        }

        // Intercepts 'Coalesce' virtual method...
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 2 &&
            string.Compare(name, "Coalesce", !engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(engine, node.LambdaArguments[0]);
            var right = Parse(engine, node.LambdaArguments[1]);

            return new DbTokenCoalesce(left, right);
        }

        // Intercepts 'Ternary' virtual method...
        if (node.LambdaHost is LambdaNodeArgument &&
            node.LambdaArguments.Count == 3 &&
            string.Compare(name, "Ternary", !engine.CaseSensitiveNames) == 0)
        {
            var left = Parse(engine, node.LambdaArguments[0]);
            var middle = Parse(engine, node.LambdaArguments[1]);
            var right = Parse(engine, node.LambdaArguments[2]);

            return new DbTokenTernary(left, middle, right);
        }

        // Intercepts 'Convert' and 'Cast' virtual methods...
        if (node.LambdaHost is LambdaNodeArgument && (
            string.Compare(name, "Convert", !engine.CaseSensitiveNames) == 0 ||
            string.Compare(name, "Cast", !engine.CaseSensitiveNames) == 0))
        {
            // x => x.Convert<T>(target)...
            if (node.LambdaGenericArguments.Count == 1 &&
                node.LambdaArguments.Count == 1)
            {
                var type = node.LambdaGenericArguments[0];
                var target = Parse(engine, node.LambdaArguments[0]);

                return new DbTokenConvert.ToType(type, target);
            }

            // x => x.Convert(type|spec, target)...
            if (node.LambdaArguments.Count == 2 &&
                node.LambdaArguments[0] is LambdaNodeValue value)
            {
                var target = Parse(engine, node.LambdaArguments[1]);
                switch (value.LambdaValue)
                {
                    case Type vtype: return new DbTokenConvert.ToType(vtype, target);
                    case string vspec: return new DbTokenConvert.ToSpec(vspec, target);
                }
            }
        }

        // Standard case...
        var host = Parse(engine, node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(engine, x));

        return new DbTokenMethod(host, name, node.LambdaGenericArguments, items);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeSetter node)
    {
        var target = Parse(engine, node.LambdaTarget);
        var value = Parse(engine, node.LambdaValue);

        return new DbTokenSetter(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeTernary node)
    {
        var left = Parse(engine, node.LambdaLeft);
        var middle = Parse(engine, node.LambdaMiddle);
        var right = Parse(engine, node.LambdaRight);

        return new DbTokenTernary(left, middle, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeUnary node)
    {
        var target = Parse(engine, node.LambdaTarget);
        return new DbTokenUnary(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken Parse(IEngine engine, LambdaNodeValue node) => node.LambdaValue switch
    {
        IDbToken item => item,
        LambdaNode item => Parse(engine, item),
        ICommand item => new DbTokenCommand(item),

        Delegate => throw new ArgumentException(
            "Cannot use delegates as the value of tokens.")
            .WithData(node),

        _ => new DbTokenValue(node.LambdaValue)
    };
}