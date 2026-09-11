namespace Yotei.ORM.Internals;

// ========================================================
public partial record DbTokenVisitor
{
    /// <summary>
    /// Translates the given dynamic lambda expression into a chain of tokens, and returns its
    /// appropriate database representation.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var token = new DbLambdaParser(Engine).Parse(expression);
        var info = Visit(token);
        return info;
    }

    /// <summary>
    /// Visits the chains of tokens represented by the given last-most one, and returns the
    /// appropriate database representation.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return token switch
        {
            DbTokenArgument item => VisitToken(item),
            DbTokenBinary item => VisitToken(item),
            DbTokenChain item => VisitToken(item),
            DbTokenCoalesce item => VisitToken(item),
            DbTokenCommandInfo item => VisitToken(item),
            DbTokenConvert item => VisitToken(item),
            DbTokenIdentifier item => VisitToken(item),
            DbTokenIndexed item => VisitToken(item),
            DbTokenInvoke item => VisitToken(item),
            DbTokenLiteral item => VisitToken(item),
            DbTokenMethod item => VisitToken(item),
            DbTokenSetter item => VisitToken(item),
            DbTokenTernary item => VisitToken(item),
            DbTokenUnary item => VisitToken(item),
            DbTokenValue item => VisitToken(item),

            IEnumerable<IDbToken> item => VisitRange(item),

            _ => throw new ArgumentException("Unknown token.").WithData(token)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Visits the given collection of tokens and returns the appropriate database representation
    /// of them all joined with the current range separator. No brackets are used to wrapped that
    /// representation.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<IDbToken> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var builder = new CommandInfo.Builder(Engine);
        var needed = false;

        foreach (var token in range)
        {
            if (token is null) throw new ArgumentException(
                "Range of tokens contains null elements.")
                .WithData(range);

            if (needed && RangeSeparator is not null) builder.AddText(RangeSeparator);
            needed = true;

            if (token is IEnumerable<IDbToken> chain)
            {
                var temp = VisitRange(chain);
                builder.Add(temp);
            }
            else
            {
                var temp = Visit(token);
                builder.Add(temp);
            }
        }

        return builder;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> Argument tokens are translation artifacts with no database representation.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenArgument token)
    {
        return new CommandInfo.Builder(Engine);
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, intercepts comparisons against null right values (only if
    /// <see cref="UseNullString"/> is true), returning either "IS NULL" or "IS NOT NULL".
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenBinary token)
    {
        // Always needed...
        var left = Visit(token.Left);

        // Intercepting...
        if (token.Right is DbTokenValue value && value.Value is null && UseNullString)
        {
            var temp = ToValueString(null);

            switch (token.Operation)
            {
                case ExpressionType.Equal:
                    left.ReplaceText($"({left.Text} IS {temp})");
                    return left;

                case ExpressionType.NotEqual:
                    left.ReplaceText($"({left.Text} IS NOT {temp})");
                    return left;
            }
        }

        // Supported operations...
        var op = token.Operation switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",

            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Modulo => "%",
            ExpressionType.Power => "^",

            ExpressionType.And => "AND",
            ExpressionType.Or => "OR",

            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",

            _ => throw new UnreachableException("Unsupported binary operation.").WithData(token)
        };

        // Finishing...
        var right = Visit(token.Right);

        left.ReplaceText($"{left.Text} {op} ");
        left.Add(right);
        left.AddText(")");
        return left;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenChain token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCoalesce token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCommandInfo token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenConvert token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIdentifier token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIndexed token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenInvoke token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenLiteral token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenMethod token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenSetter token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenTernary token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenUnary token)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenValue token)
    {
        throw null;
    }
}