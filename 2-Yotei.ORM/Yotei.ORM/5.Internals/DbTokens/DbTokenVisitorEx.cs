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
    public virtual ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
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
        var host = Visit(token.Host);
        var darg = token.GetArgument();
        var name = token.Identifier.UnwrappedValue.NullWhenDynamicName(darg, caseSensitive: true);

        // Wrapping if needed...
        if (name is not null && UseTerminators && Engine.UseTerminators)
            name = name.Wrap(Engine.LeftTerminator, Engine.RightTerminator);

        // Joining with previous with a dot, except if such is an argument (an artifact with no
        // representation), or an invoke (which is used to inject arbitrary contents, that shall
        // take care of dots if needed)...

        if (token.Host is not DbTokenArgument)
        {
            name = host.Text.Length == 0 || token.Host is DbTokenInvoke
                ? name
                : $".{name}";
        }

        // Removing redundant dots...
        name = host.Text + name;
        while (name.StartsWith('.') && name.Length > 1) name = name[1..];

        // Finishing...
        host.ReplaceText(name);
        return host;
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
    /// any separator, and then adds it to any previous contents. This process provide a way to
    /// inject arbitrary contents into the returned text element when needed.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitInvoke(DbTokenInvoke token)
    {
        var host = Visit(token.Host);
        var temp = ToNullSeparatorVisitor();
        var args = temp.VisitRange(token.Arguments);

        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> By convention, values carried by literal tokens ARE NOT captured as arguments,
    /// and are just injected into the returned text element.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitLiteral(DbTokenLiteral token)
    {
        return new CommandInfo.Builder(Engine, token.Value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> If the method name is strictly the same as the name of the dynamic argument, as
    /// in 'x => ...x(...)...', then it is treated as an invoke operation. This mechanism is
    /// used because 'x => x.Any()' is interpreted as a method, whereas 'x => x.Any.x(...)' is
    /// now interpreted as an invoke one.
    /// <br/> In addition, this method intercepts a number of 'virtual' method invocations, and
    /// translates them into the appropriate database constructs. By default, the intercepted
    /// names are:
    /// <br/>- Argument-level: NOT, COUNT, CAST, CONVERT.
    /// <br/>- Member-level: AS, IN, NOTIN, BETWEEN, LIKE, NOTLIKE.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitMethod(DbTokenMethod token)
    {
        // Intercepting invoke-alike tokens...
        var name = token.Name;
        var darg = token.GetArgument();

        if (darg is not null && darg.Name == name)
        {
            if (token.TypeArguments.Length != 0) throw new ArgumentException(
                "Invoke-alike methods do not support generic type arguments.")
                .WithData(token);

            return Visit(new DbTokenInvoke(token.Host, token.Arguments));
        }

        // Other methods...
        var host = Visit(token.Host);
        var upper = name.ToUpper();
        ICommandInfo.IBuilder temp;
        ICommandInfo.IBuilder other;
        DbTokenChain chain;

        // Argument-level methods...
        if (token.Host is DbTokenArgument)
        {
            switch (upper)
            {
                case "NOT":
                    if (token.Arguments.Count != 1) Throw($"NOT(expr) requires just 1 argument.");
                    temp = Visit(token.Arguments[0]);
                    temp.ReplaceText($"(NOT {temp.Text})");
                    return temp;

                case "COUNT":
                    if (IsEmptyOrSoleAsterisk(token.Arguments))
                        return new CommandInfo.Builder(Engine, "COUNT(*)");

                    temp = this.ToCommaVisitor().VisitRange(token.Arguments);
                    temp.ReplaceText($"COUNT({temp.Text})");
                    return temp;

                case "CAST":
                    if (token.TypeArguments.Length == 1)
                    {
                        if (token.Arguments.Count != 1) Throw($"CAST<type>(expr) requires just 1 argument.");
                        temp = Visit(token.Arguments[0]);
                        name = token.TypeArguments[0].EasyName();
                        temp.ReplaceText($"CAST({name} AS {temp.Text})");
                        return temp;
                    }
                    if (token.TypeArguments.Length == 0)
                    {
                        if (token.Arguments.Count != 2) Throw($"CAST(expr, type) requires just 2 arguments.");
                        temp = Visit(token.Arguments[0]);
                        temp.ReplaceText($"CAST({temp.Text} AS ");

                        other = this.ToRawVisitor().Visit(token.Arguments[1]);
                        temp.Add(other);
                        temp.Add(")");
                        return temp;
                    }
                    Throw("Too many generic arguments for a CAST<type>(expr) method.");
                    break;

                case "CONVERT":
                    if (token.Arguments.Count != 2) Throw($"CONVERT(type, expr) requires just 2 arguments.");
                    temp = this.ToRawVisitor().Visit(token.Arguments[0]);
                    temp.ReplaceText($"CONVERT({temp.Text}, ");

                    other = Visit(token.Arguments[1]);
                    temp.Add(other);
                    temp.Add(")");
                    return temp;
            }
        }

        // Member-level methods...
        else
        {
            switch (upper)
            {
                case "AS":
                    if (token.Arguments.Count == 0) Throw($"AS(expr, ...) requires at least 1 argument.");
                    name = this.ChainToAlias(token.Arguments);
                    host.Add($" AS {name}");
                    return host;

                case "IN":
                    if (token.Arguments.Count == 0) Throw($"IN(expr, ...) requires at least 1 argument.");
                    chain = TryExpandFirstAlone(token.Arguments);
                    temp = ToCommaVisitor().VisitRange(chain);
                    host.Add(" IN (");
                    host.Add(temp);
                    host.Add(")");
                    return host;

                case "NOTIN":
                    if (token.Arguments.Count == 0) Throw($"IN(expr, ...) requires at least 1 argument.");
                    chain = TryExpandFirstAlone(token.Arguments);
                    temp = ToCommaVisitor().VisitRange(chain);
                    host.Add(" NOT IN (");
                    host.Add(temp);
                    host.Add(")");
                    return host;

                case "BETWEEN":
                    if (token.Arguments.Count != 2) Throw($"BETWEEN(expr, expr) requires 2 arguments.");
                    temp = Visit(token.Arguments[0]);
                    host.Add($" BETWEEN ({temp.Text} AND ", temp.Parameters);

                    other = Visit(token.Arguments[1]);
                    host.Add(other);
                    host.Add(")");
                    return host;

                case "LIKE":
                    if (token.Arguments.Count != 1) Throw($"LIKE(expr) requires just 1 argument.");
                    temp = Visit(token.Arguments[0]);
                    host.Add($" LIKE {temp.Text}", temp.Parameters);
                    return host;

                case "NOTLIKE":
                    if (token.Arguments.Count != 1) Throw($"LIKE(expr) requires just 1 argument.");
                    temp = Visit(token.Arguments[0]);
                    host.Add($" NOT LIKE {temp.Text}", temp.Parameters);
                    return host;
            }
        }

        // Default...
        if (token.Host is not DbTokenArgument and not DbTokenInvoke) host.Add(".");
        host.Add(name);

        if (token.TypeArguments.Length > 0)
        {
            host.Add("<");
            host.Add(string.Join(", ", token.TypeArguments.Select(x => x.EasyName())));
            host.Add(">");
        }

        temp = this.ToCommaVisitor().VisitRange(token.Arguments);
        host.Add("(");
        host.Add(temp);
        host.Add(")");
        return host;

        // Exception helper...
        void Throw(string msg) => new ArgumentException(msg).WithData(token);
    }

    /// <summary>
    /// Determines if the given chain is either an empty one, or consist in just one element
    /// with an asterisk-alike value.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static bool IsEmptyOrSoleAsterisk(DbTokenChain chain)
    {
        if (chain.Count == 0) return true;

        if (chain.Count == 1 &&
            chain[0] is DbTokenValue value && (
            (value.Value is char c && c == '*') || (value.Value is string s && s == "*")))
            return true;

        return false;
    }

    /// <summary>
    /// If the first and unique element of the given chain is an enumerable one (except strings),
    /// expands that element and returns that expansion. Otherwise, returns the original instance.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public DbTokenChain TryExpandFirstAlone(DbTokenChain chain)
    {
        if (chain.Count == 1 &&
            chain[0] is DbTokenValue value &&
            value.Value is not string &&
            value.Value is IEnumerable iter)
        {
            var builder = new DbTokenChain.Builder();

            foreach (var item in iter)
            {
                switch (item)
                {
                    case IDbToken token:
                        builder.Add(token);
                        break;

                    case LambdaNode node:
                        var other = DbLambdaParser.Parse(Engine, node);
                        builder.Add(other);
                        break;

                    default:
                        builder.Add(new DbTokenValue(item));
                        break;
                }
            }

            chain = builder.CreateInstance();
        }

        return chain;
    }

    /// <summary>
    /// Invoked to build an alias from the contents of the given chain which, by default, are
    /// joined without any separators among them. Throws an exception if the alias resolves
    /// into a null or empty literal.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public string ChainToAlias(DbTokenChain chain)
    {
        chain.ThrowWhenNull();

        var visitor = ToRawVisitor();
        var builder = visitor.VisitRange(chain);

        var id = new IdentifierPart(Engine, builder.Text);
        var name = Engine.UseTerminators ? id.Value : id.UnwrappedValue;

        return name
            ?? throw new ArgumentException("Generated alias resolves into null.").WithData(chain);
    }

    /// <summary>
    /// Reduces the given token to a string literal.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public string TokenToLiteral(IDbToken token)
    {
        token.ThrowWhenNull();

        var visitor = ToRawVisitor();
        var builder = visitor.Visit(token);
        return builder.Text;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, just wraps the setter operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitSetter(DbTokenSetter token)
    {
        var target = Visit(token.Target);
        var value = Visit(token.Value);

        target.ReplaceText($"({target.Text} = ");
        target.Add($"{value.Text})", value.Parameters);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, produces a 'IF (left) THEN (middle) ELSE (right)' text
    /// construct.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitTernary(DbTokenTernary token)
    {
        var left = Visit(token.Left);
        var middle = Visit(token.Middle);
        var right = Visit(token.Right);

        left.ReplaceText($"IF (({left.Text}) ");
        left.Add($"THEN ({middle.Text}) ", middle.Parameters);
        left.Add($"ELSE ({right.Text}))", right.Parameters);
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by default, just wraps the unary operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitUnary(DbTokenUnary token)
    {
        var target = Visit(token.Target);

        switch (token.Operation)
        {
            case ExpressionType.Not: target.ReplaceText($"(NOT {target.Text})"); break;
            case ExpressionType.Negate: target.ReplaceText($"-({target.Text})"); break;
            default:
                throw new ArgumentException("Unsupported unary operation.").WithData(token);
        }

        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/>- This method, by default, captures the value content as a command argument of the
    /// returned instance. If <see cref="CaptureValues"/> is not enabled, then their text string
    /// representation is injected instead.
    /// <br/>- Null values might not be captured is <see cref="UseNullString"/> is enabled, when
    /// their null string representation is injected instead.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICommandInfo.IBuilder VisitValue(DbTokenValue token)
    {
        // Null-alike...
        if (token.Value is null && UseNullString)
        {
            var temp = ToValueString(null);
            var info = new CommandInfo.Builder(Engine, temp);
            return info;
        }

        // Capturing values...
        if (CaptureValues)
        {
            var prefix = Engine.ParameterPrefix;
            var name = $"{prefix}0";
            var par = new Parameter(name, token.Value);
            var info = new CommandInfo.Builder(Engine, "{0}", [par]);
            return info;
        }

        // Injecting value representation...
        else
        {
            var temp = ToValueString(token.Value);
            var info = new CommandInfo.Builder(Engine, temp);
            return info;
        }
    }
}