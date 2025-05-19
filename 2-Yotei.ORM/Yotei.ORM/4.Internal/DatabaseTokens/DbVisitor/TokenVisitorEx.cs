namespace Yotei.ORM.Internal;

// ========================================================
partial record class TokenVisitor
{
    /// <summary>
    /// Visits the chain of tokens obtained from parsing the given dynamic lambda expression,
    /// and returns the command info extracted from it.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
    {
        expression.ThrowWhenNull();

        var parser = new DbLambdaParser(Engine);
        var token = parser.Parse(expression);
        var info = Visit(token);
        return info;
    }

    /// <summary>
    /// Visits the chain of tokens represented by the given one as the last node in that chain,
    /// and returns the command info extracted from it.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandInfo.IBuilder Visit(DbToken token)
    {
        return token switch
        {
            DbTokenArgument item => Parse(item),
            DbTokenBinary item => Parse(item),
            DbTokenChain item => Parse(item),
            DbTokenCoalesce item => Parse(item),
            DbTokenCommand item => Parse(item),
            DbTokenConvert item => Parse(item),
            DbTokenIdentifier item => Parse(item),
            DbTokenIndexed item => Parse(item),
            DbTokenInvoke item => Parse(item),
            DbTokenLiteral item => Parse(item),
            DbTokenMethod item => Parse(item),
            DbTokenSetter item => Parse(item),
            DbTokenTernary item => Parse(item),
            DbTokenUnary item => Parse(item),
            DbTokenValue item => Parse(item),

            _ => throw new ArgumentException("Unknown token.").WithData(token)
        };
    }

    /// <summary>
    /// Invoked to visit the given range of tokens, joining the results with the current
    /// <see cref="RangeSeparator"/>.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ICommandInfo.IBuilder Visit(IEnumerable<DbToken> range)
    {
        var builder = new CommandInfo.Builder(Engine);
        var num = false;

        foreach (var token in range)
        {
            if (token is null) throw new ArgumentException(
                "Range of tokens contains null elements.")
                .WithData(range);

            if (num && RangeSeparator != null) builder.Add(RangeSeparator);
            num = true;

            if (token is IEnumerable<DbToken> chain)
            {
                var temp = Visit(chain);
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
    /// Invoked to parse the given token.
    /// <br/> Arguments are considered translation artifacts with no representation in database
    /// commands. So, they parse to empty strings.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(
        DbTokenArgument _)
        => new CommandInfo.Builder(Engine);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method intercepts right-null values translating them into the appropriate
    /// database construct.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenBinary token)
    {
        // Always need the left-part...
        var left = Visit(token.Left);

        // Intercepting right-null values...
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

        left.ReplaceText($"({left.Text} {op} ");
        left.Add($"{right.Text})", right.Parameters);
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenChain token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenCoalesce token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenCommand token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenConvert token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenIdentifier token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenIndexed token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenInvoke token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenLiteral token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenMethod token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenSetter token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenTernary token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenUnary token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenValue token)
    {
        throw null;
    }
}