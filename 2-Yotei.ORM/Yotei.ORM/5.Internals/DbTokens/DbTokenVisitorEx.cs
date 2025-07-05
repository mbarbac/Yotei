namespace Yotei.ORM.Internals;

// ========================================================
partial record class DbTokenVisitor
{
    /// <summary>
    /// Visit the chain of tokens obtained from the given dynamic lambda expression and returns
    /// the command info extracted from it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit<T>(Func<dynamic, T> expression)
    {
        expression.ThrowWhenNull();

        var token = DbLambdaParser.Parse(Engine, expression);
        var info = Visit(token);
        return info;
    }

    /// <summary>
    /// Visit the chain of tokens represented by the top-most given one, and returns the command
    /// info extracted from it.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        token.ThrowWhenNull();

        return token switch
        {
            DbTokenArgument item => VisitArgument(item),
            DbTokenBinary item => VisitBinary(item),
            DbTokenChain item => VisitChain(item),
            DbTokenCoalesce item => VisitCoalesce(item),
            DbTokenCommand item => VisitCommand(item),
            DbTokenConvert item => VisitConvert(item),
            DbTokenIdentifier item => VisitIdentifier(item),
            DbTokenIndexed item => VisitIndexed(item),
            DbTokenInvoke item => VisitInvoke(item),
            DbTokenLiteral item => VisitLiteral(item),
            DbTokenMethod item => VisitMethod(item),
            DbTokenSetter item => VisitSetter(item),
            DbTokenTernary item => VisitTernary(item),
            DbTokenUnary item => VisitUnary(item),
            DbTokenValue item => VisitValue(item),

            IEnumerable<IDbToken> item => VisitRange(item),

            _ => throw new ArgumentException("Unknown token.").WithData(token)
        };
    }

    /// <summary>
    /// Invoked to visit the given range of tokens. This method joins their text results with the
    /// current range separator.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<IDbToken> range)
    {
        var builder = new CommandInfo.Builder(Engine);
        var one = false;

        foreach (var token in range)
        {
            if (token is null) throw new ArgumentException(
                "Range of tokens contains null elements.").WithData(range);

            if (one && RangeSeparator is not null) builder.Add(RangeSeparator);
            one = true;

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
    /// <br/> Arguments are considered translation artifacts with no database representation. So,
    /// by default, this method just returns an empty instance.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitArgument(DbTokenArgument token)
    {
        return new CommandInfo.Builder(Engine);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, intercepts equality comparisons against null right values
    /// and returns the appropriate constructs "IS NULL" or "IS NOT NULL". Otherwise, returns the
    /// default operation representation.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitBinary(DbTokenBinary token)
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
        left.Add(right);
        left.Add(")");
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, joins the text results of each of its elements with the
    /// current range separator.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitChain(DbTokenChain token)
    {
        return VisitRange(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, produces a TSQL-alike 'COALESCE(left, righ)' element.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitCoalesce(DbTokenCoalesce token)
    {
        var left = Visit(token.Left);
        var right = Visit(token.Right);

        left.ReplaceText($"COALESCE({left.Text}, ");
        left.Add(right);
        left.Add(")");
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, wraps the not-iterable text element of the given command
    /// between rounded brackets. If the command is an empty one, then an empty instance is
    /// returned instead.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitCommand(DbTokenCommand token)
    {
        var info = token.Command.GetCommandInfo();

        if (info.IsEmpty) return new CommandInfo.Builder(Engine);
        else
        {
            var builder = new CommandInfo.Builder(info);
            var str = builder.Text;
            str = str.UnWrap('(', ')').Wrap('(', ')');

            builder.ReplaceText($"{str}");
            return builder;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, produces a 'CAST(target AS type)' construct, where type
    /// is either its text specification, or the easy name of the given <see cref="Type"/> one.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitConvert(DbTokenConvert token)
    {
        var target = Visit(token.Target);

        // Actual type given...
        if (token is DbTokenConvert.ToType totype)
        {
            var type = totype.Type.EasyName();
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {type})", target.Parameters);
            return builder;
        }

        // Type specification as text...
        else if (token is DbTokenConvert.ToSpec tospec)
        {
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {tospec.Type})", target.Parameters);
            return builder;
        }

        // Unkown...
        else throw new ArgumentException("Unknown conversion token.").WithData(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> If the identifier name is strictly the same as the dynamic argument one, then this
    /// method interprets it as an empty one. It also removes redundant head dots from its name.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitIdentifier(DbTokenIdentifier token)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, just joins the text results of the indexed arguments with
    /// comma separators, and wraps them all between squared brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitIndexed(DbTokenIndexed token)
    {
        var host = Visit(token.Host);
        var temp = ToCommaVisitor();
        var args = temp.VisitRange(token.Indexes);

        args.ReplaceText($"[{args.Text}]");
        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, joins the text representation of its parameters without
    /// any separator, and then adds it to any previous contents. 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitInvoke(DbTokenInvoke token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitLiteral(DbTokenLiteral token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitMethod(DbTokenMethod token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitSetter(DbTokenSetter token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitTernary(DbTokenTernary token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitUnary(DbTokenUnary token) { throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitValue(DbTokenValue token) { throw null; }
}