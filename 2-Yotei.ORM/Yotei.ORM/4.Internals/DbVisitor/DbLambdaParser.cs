using System.Runtime.InteropServices.Marshalling;

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
/// text for the underlying database engine, and the captured SQL arguments.</remarks>
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
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations in that expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public IDbToken Parse<T>(Func<dynamic, T> expression)
    {
        var parser = LambdaParser.Parse(expression);
        var node = parser.Result;
        var method = expression.GetMethodInfo();

        // Special case for 'x => "..."' syntax...
        if (method.ReturnType == typeof(string) &&
            node is LambdaNodeValue value && value.LambdaValue is string str)
        {
            return new DbTokenLiteral(str);
        }

        // Standard 'x => ...' syntax...
        else
        {
            var token = Parse(node);
            return token;
        }
    }

    /// <summary>
    /// Parses the given lambda node and returns the last database-alike token in the chain
    /// that contains the dynamic operations in that node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public IDbToken Parse(LambdaNode node)
    {
        node.ThrowWhenNull();

        var temp = node switch
        {
            LambdaNodeArgument item => Parse(item),
            LambdaNodeBinary item => Parse(item),
            LambdaNodeCoalesce item => Parse(item),
            LambdaNodeConvert item => Parse(item),
            LambdaNodeIndexed item => Parse(item),
            LambdaNodeInvoke item => Parse(item),
            LambdaNodeMember item => Parse(item),
            LambdaNodeMethod item => Parse(item),
            LambdaNodeSetter item => Parse(item),
            LambdaNodeTernary item => Parse(item),
            LambdaNodeUnary item => Parse(item),
            LambdaNodeValue item => Parse(item),

            _ => throw new ArgumentException("Unknown node.").WithData(node)
        };

        if (temp is DbTokenChain chain) temp = chain.Reduce();
        return temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    static DbTokenArgument Parse(LambdaNodeArgument node) => new(node.LambdaName);

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenBinary Parse(LambdaNodeBinary node)
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
    IDbToken Parse(LambdaNodeCoalesce node)
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

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenConvert.ToType Parse(LambdaNodeConvert node)
    {
        var target = Parse(node.LambdaTarget);
        return new DbTokenConvert.ToType(node.LambdaType, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenIndexed Parse(LambdaNodeIndexed node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaIndexes.Select(x => Parse(x));

        return new(host, items);
    }

    /// <summary>
    /// Parses the given node.
    /// <br/>- Single command-alike arguments '(cmd)' are translated into a command instances.
    /// <br/>- Unique '(str)' string arguments are translated into literal **arguments**...
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken Parse(LambdaNodeInvoke node)
    {
        var host = Parse(node.LambdaHost);
        var items = node.LambdaArguments.Select(x => Parse(x)).ToList();

        if (items.Count == 1) // Intercepting special cases...
        {
            // Command-alike tokens...
            if (items[0] is DbTokenCommand command) return command;

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
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenIdentifier Parse(LambdaNodeMember node)
    {
        var host = Parse(node.LambdaHost);
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        var id = name == null ? new IdentifierPart(Engine) : new IdentifierPart(Engine, name);
        return new(host, id);
    }

    /// <summary>
    /// Parses the given node.
    /// <br/> Translates 'x => x(...)' and 'x => x.Any.x(...)' to invoke.
    /// <br/> Intercepts 'x => x.Coalesce(...)' virtual method.
    /// <br/> Intercepts 'x => x.Ternary(...)' virtual method.
    /// <br/> Intercepts 'x => x.Convert(...)' and 'x => x.Cast(...)' virtual methods.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken Parse(LambdaNodeMethod node)
    {
        var darg = node.GetArgument();
        var name = node.LambdaName.NullWhenDynamicName(darg, Engine.CaseSensitiveNames);

        // 'x => x(...)' and 'x => x.Any.x(...)' to invoke...
        if (name == null)
        {
            if (node.LambdaGenericArguments.Count != 0) throw new ArgumentException(
                "Cannot use type arguments with invoke tokens.")
                .WithData(node);

            var invoke = new LambdaNodeInvoke(node.LambdaHost, node.LambdaArguments);
            return Parse(invoke);
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

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenSetter Parse(LambdaNodeSetter node)
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
    DbTokenTernary Parse(LambdaNodeTernary node)
    {
        var left = Parse(node.LambdaLeft);
        var middle = Parse(node.LambdaMiddle);
        var right = Parse(node.LambdaRight);

        return new(left, middle, right);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    DbTokenUnary Parse(LambdaNodeUnary node)
    {
        var target = Parse(node.LambdaTarget);
        return new(node.LambdaOperation, target);
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IDbToken Parse(LambdaNodeValue node) => node.LambdaValue switch
    {
        IDbToken item => item,
        LambdaNode item => Parse(item),
        ICommand item => new DbTokenCommand(item),

        Delegate => throw new ArgumentException(
            "Cannot use delegates as the value of tokens.")
            .WithData(node),

        _ => new DbTokenValue(node.LambdaValue)
    };
}