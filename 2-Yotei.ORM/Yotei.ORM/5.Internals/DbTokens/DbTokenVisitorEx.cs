namespace Yotei.ORM.Internals;

partial record class DbTokenVisitor
{
    /// <summary>
    /// Visits the chain of tokens obtained from parsing the given dynamic lambda expression, and
    /// returns the command info extracted from it.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
    {
        throw null;
    }

    /// <summary>
    /// Visits the chain of tokens represented by the given top-most one, and returns the command
    /// info extracted from it.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given range of tokens and return the command info that represents
    /// them all, joining the results with the current range separator.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<IDbToken> range)
    {
        throw null;
    }


    // ====================================================

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenArgument token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenBinary token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenChain token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCoalesce token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCommand token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCommandInfo token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(ICommandInfo.IBuilder token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenConvert token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIdentifier token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIndexed token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenInvoke token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenLiteral token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenMethod token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenSetter token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenTernary token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenUnary token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenValue token)
    {
        throw null;
    }
}