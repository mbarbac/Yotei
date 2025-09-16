namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last database-alike
/// token in the chain that contains the dynamic operations of that expression.
/// </summary>
/// The objective of this class is to provide a fast parsing of dynamic lambda expressions into
/// database alike-tokens. Later, visitor instances can translate those token chains into the
/// command-info objects that contains the appropriate text and arguments for the underlying
/// database.
public static class DbLambdaParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations of that expression.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static IDbToken Parse(IEngine engine, Func<dynamic, object?> expression)
    {
    }

    /// <summary>
    /// Parses the given dynamic lambda node that represents a chain of dynamic operations, and
    /// returns the last database-alike token in the chain that contains the dynamic operations
    /// of that lambda token.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IDbToken Parse(IEngine engine, LambdaNode node)
    {
    }

    // ====================================================

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeArgument node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeBinary node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeCoalesce node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeConvert node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeIndexed node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeInvoke node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeMember node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeMethod node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeSetter node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeTernary node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeUnary node)
    {
        throw null;
    }

    /// <summary>
    /// Parses the given node.
    /// </summary>
    static IDbToken ParseNode(IEngine engine, LambdaNodeValue node)
    {
        throw null;
    }
}