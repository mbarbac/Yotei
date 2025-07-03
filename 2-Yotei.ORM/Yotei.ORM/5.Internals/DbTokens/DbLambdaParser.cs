#pragma warning disable IDE0079
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
/// text for the underlying database engine, and the captured SQL arguments.
/// </remarks>
public static class DbLambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations in that expression.
    /// <br/>- Standard syntax: 'x => x...'
    /// <br/>- Alternate syntax: 'x => "..."', where the string contents are taken as literal ones.
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
            node is LambdaNodeValue value &&
            value.LambdaValue is string str)
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
    /// 
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static IDbToken Parse(IEngine engine, LambdaNode node)
    {
        node.ThrowWhenNull();

        var temp = node switch
        {
            LambdaNodeArgument item => ParseArgument(engine, item),
            LambdaNodeBinary item => ParseBinary(engine, item),
            LambdaNodeCoalesce item => ParseCoalesce(engine, item),
            LambdaNodeConvert item => ParseConvert(engine, item),
            LambdaNodeIndexed item => ParseIndexed(engine, item),
            LambdaNodeInvoke item => ParseInvoke(engine, item),
            LambdaNodeMember item => ParseMember(engine, item),
            LambdaNodeMethod item => ParseMethod(engine, item),
            LambdaNodeSetter item => ParseSetter(engine, item),
            LambdaNodeTernary item => ParseTernary(engine, item),
            LambdaNodeUnary item => ParseUnary(engine, item),
            LambdaNodeValue item => ParseValued(engine, item),

            _ => throw new ArgumentException("Unknown node.").WithData(node)
        };

        if (temp is DbTokenChain chain) temp = chain.Reduce();
        return temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseArgument(IEngine _, LambdaNodeArgument node)
    {
        return new DbTokenArgument(node.LambdaName);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseBinary(IEngine engine, LambdaNodeBinary node)
    {
        var left = Parse(engine, node.LambdaLeft);
        var right = Parse(engine, node.LambdaRight);

        return new DbTokenBinary(left, node.LambdaOperation, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseCoalesce(IEngine engine, LambdaNodeCoalesce node)
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
    static IDbToken ParseConvert(IEngine engine, LambdaNodeConvert node)
    {
        var target = Parse(engine, node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseIndexed(IEngine engine, LambdaNodeIndexed node)
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
    static IDbToken ParseInvoke(IEngine engine, LambdaNodeInvoke node)
    {
        var host = Parse(engine, node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(engine, x)).ToList();

        if (items.Count == 1) // Intercepting special cases...
        {
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
    static IDbToken ParseMember(IEngine engine, LambdaNodeMember node)
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
    static IDbToken ParseMethod(IEngine engine, LambdaNodeMethod node)
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
            return ParseInvoke(engine, invoke);
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
    static IDbToken ParseSetter(IEngine engine, LambdaNodeSetter node)
    {
        var target = Parse(engine, node.LambdaTarget);
        var value = Parse(engine, node.LambdaValue);

        return new DbTokenSetter(target, value);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseTernary(IEngine engine, LambdaNodeTernary node)
    {
        var left = Parse(engine, node.LambdaLeft);
        var middle = Parse(engine, node.LambdaMiddle);
        var right = Parse(engine, node.LambdaRight);

        return new DbTokenTernary(left, middle, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseUnary(IEngine engine, LambdaNodeUnary node)
    {
        var target = Parse(engine, node.LambdaTarget);
        return new DbTokenUnary(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseValued(IEngine engine, LambdaNodeValue node) => node.LambdaValue switch
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