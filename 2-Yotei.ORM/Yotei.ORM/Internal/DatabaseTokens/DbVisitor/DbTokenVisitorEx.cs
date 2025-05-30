﻿namespace Yotei.ORM.Internal;

// ========================================================
partial record class DbTokenVisitor
{
    /// <summary>
    /// Visits the chain of tokens obtained from parsing the given dynamic lambda expression,
    /// and returns the command info extracted from it.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
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
    public virtual ICommandInfo.IBuilder Visit(DbToken token)
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

            IEnumerable<DbToken> item => VisitRange(item),

            _ => throw new ArgumentException("Unknown token.").WithData(token)
        };
    }

    /// <summary>
    /// Invoked to visit the given range of tokens, joining the results with the current range
    /// separator <see cref="RangeSeparator"/>.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<DbToken> range)
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
    /// Returns a clone of this instance with the appropriate separator for range elements.
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToCommaRangeVisitor() => this with { RangeSeparator = ", " };

    /// <summary>
    /// Returns a clone of this instance that uses no separator between range elements.
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToNoSeparatorVisitor() => this with { RangeSeparator = null };

    /// <summary>
    /// Returns a clone of this instance with all its settings to <c>false</c> or <c>null</c>.
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToRawVisitor() => this with
    {
        UseNullString = false,
        CaptureValues = false,
        ConvertValues = false,
        UseQuotes = false,
        UseTerminators = false,
        RangeSeparator = null,
    };

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> Arguments are considered translation artifacts with no representation in database
    /// commands. So, they parse to empty strings and hence why this method is not virtual.
    /// </summary>
    protected ICommandInfo.IBuilder Parse(DbTokenArgument _)
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
        left.Add(right);
        left.Add(")");
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method visits the tokens in the given chain, and joins them using the current
    /// range separator <see cref="RangeSeparator"/>.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenChain token)
    {
        return VisitRange(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenCoalesce token)
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
    /// Invoked to parse the given token.
    /// <br/> If the command is not an empty one, his method wraps between rounded brackets
    /// the command's text. If it is empty, then returns an empty builder.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenCommand token)
    {
        if (token.CommandInfo.IsEmpty)
        {
            var engine = token.CommandInfo.Engine;
            return new CommandInfo.Builder(engine);
        }
        else
        {
            var builder = new CommandInfo.Builder(token.CommandInfo);

            builder.ReplaceText($"({builder.Text})");
            return builder;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method translates the conversion to a database 'CAST(source AS type)' one
    /// by default.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenConvert token)
    {
        var target = Visit(token.Target);

        if (token is DbTokenConvert.ToType toType) // Actual type given...
        {
            var type = toType.Type.EasyName();
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {type})", target.Parameters);
            return builder;
        }
        else if (token is DbTokenConvert.ToSpec toSpec) // Type given as text...
        {
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {toSpec.Type})", target.Parameters);
            return builder;
        }
        else
        {
            throw new ArgumentException("Unsupported conversion token.").WithData(token);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method parses the complete chain that ends with the given identifier.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenIdentifier token)
    {
        var host = Visit(token.Host);

        var darg = token.GetArgument();
        var name = token.Identifier.UnwrappedValue.NullWhenDynamicName(darg, caseSensitive: true);
        name ??= string.Empty;

        if (name.Length > 0 && UseTerminators)
            name = name.Wrap(Engine.LeftTerminator, Engine.RightTerminator);

        if (token.Host is not DbTokenArgument)
        {
            name = host.Text.Length == 0 || token.Host is DbTokenInvoke
                ? name
                : $".{name}";
        }

        name = host.Text + name;
        while (name.StartsWith('.') && name.Length > 1) name = name[1..];

        host.ReplaceText(name);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method parses the complete chain that ends with the given indexes.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenIndexed token)
    {
        var host = Visit(token.Host);
        var temp = ToCommaRangeVisitor();
        var args = temp.VisitRange(token.Indexes);

        args.ReplaceText($"[{args.Text}]");
        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method, by definition, joins the invoke arguments with no separators.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenInvoke token)
    {
        var host = Visit(token.Host);
        var temp = ToNoSeparatorVisitor();
        var args = temp.VisitRange(token.Arguments);

        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> Literal tokens are NOT captured as arguments and their text just injected into
    /// the command's one.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenLiteral token)
    {
        return new CommandInfo.Builder(Engine, token.Value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method intercepts some virtual methods, and translates them to the appropriate
    /// database constructs. If the name of the virtual method is the same as the name of the
    /// dynamic argument, then it is translated into an invoke operation.
    /// <br/> Otherwise, the following virtual methods are intercepted by default:
    /// <br/> Argument-level: NOT, COUNT, CAST, CONVERT.
    /// <br/> Member-level: AS, IN, NOTIN, BETWEEN, LIKE, NOTLIKE.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenMethod token)
    {
        // Intercepting invoke-alike tokens...
        var name = token.Name;
        var darg = token.GetArgument();

        if (darg != null && string.Compare(name, darg.Name, !Engine.CaseSensitiveNames) == 0)
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

                    temp = ToCommaRangeVisitor().VisitRange(token.Arguments);
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

                        other = ToRawVisitor().Visit(token.Arguments[1]);
                        temp.Add(other);
                        temp.Add(")");
                        return temp;
                    }
                    Throw("Too many generic arguments for a CAST<type>(expr) method.");
                    break;

                case "CONVERT":
                    if (token.Arguments.Count != 2) Throw($"CONVERT(type, expr) requires just 2 arguments.");
                    temp = ToRawVisitor().Visit(token.Arguments[0]);
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
                    name = ParseAlias(token.Arguments);
                    host.Add($" AS {name}");
                    return host;

                case "IN":
                    if (token.Arguments.Count == 0) Throw($"IN(expr, ...) requires at least 1 argument.");
                    chain = TryExpand(token.Arguments);
                    temp = ToCommaRangeVisitor().VisitRange(chain);
                    host.Add(" IN (");
                    host.Add(temp);
                    host.Add(")");
                    return host;

                case "NOTIN":
                    if (token.Arguments.Count == 0) Throw($"IN(expr, ...) requires at least 1 argument.");
                    chain = TryExpand(token.Arguments);
                    temp = ToCommaRangeVisitor().VisitRange(chain);
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

        temp = ToCommaRangeVisitor().VisitRange(token.Arguments);
        host.Add("(");
        host.Add(temp);
        host.Add(")");
        return host;

        /// <summary>
        /// Exception helper
        /// </summary>
        void Throw(string msg) => new ArgumentException(msg).WithData(token);
    }

    /// <summary>
    /// Determines if the given chain is either an empty one, or consist in just one element whose
    /// value is an asterisk.
    /// </summary>
    static bool IsEmptyOrSoleAsterisk(DbTokenChain chain)
    {
        if (chain.Count == 0) return true;

        if (chain.Count == 1 &&
            chain[0] is DbTokenValue value && (
            (value.Value is char c && c == '*') ||
            (value.Value is string s && s == "*")))
            return true;

        return false;
    }

    /// <summary>
    /// Returns an alias built from visiting the elements in the given chain.
    /// </summary>
    protected virtual string ParseAlias(DbTokenChain chain)
    {
        var visitor = ToRawVisitor();
        var temp = visitor.VisitRange(chain);

        var text = temp.Text;

        if (text is not null && ((text.Contains(' ') || text.Contains('.'))))
        {
            if (!Engine.UseTerminators) throw new ArgumentException(
                "Alias contains embedded dots or spaces.")
                .WithData(text);

            text = text
                .UnWrap(Engine.LeftTerminator, Engine.RightTerminator)
                .Wrap(Engine.LeftTerminator, Engine.RightTerminator);
        }

        var id = new IdentifierPart(Engine, text);
        var name = UseTerminators && Engine.UseTerminators
            ? id.Value
            : id.UnwrappedValue;

        return name
            ?? throw new ArgumentException("Cannot generate an appropriate alias.").WithData(chain);
    }

    /// <summary>
    /// Tries to expand the first element of the given chain, provided that it is an enumerable
    /// one, and it is the unique one in that chain.
    /// </summary>
    DbTokenChain TryExpand(DbTokenChain chain)
    {
        if (chain.Count == 1)
        {
            var token = chain[0];

            if (token is DbTokenValue value &&
                value.Value is not string &&
                value.Value is IEnumerable iter)
            {
                var builder = new DbTokenChain.Builder();

                foreach (var item in iter)
                {
                    switch (item)
                    {
                        case DbToken temp:
                            builder.Add(temp);
                            break;

                        case LambdaNode temp:
                            var other = new DbLambdaParser(Engine).Parse(temp);
                            builder.Add(other);
                            break;

                        default:
                            builder.Add(new DbTokenValue(item));
                            break;
                    }
                }

                chain = builder.ToInstance();
            }
        }

        return chain;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// <br/> This method wraps the setter operation between rounded brackets.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenSetter token)
    {
        var target = Visit(token.Target);
        var value = Visit(token.Value);

        target.ReplaceText($"({target.Text} = ");
        target.Add($"{value.Text})", value.Parameters);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenTernary token)
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
    /// Invoked to parse the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenUnary token)
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
    /// Invoked to parse the given token.
    /// <br/> This method does NOT capture values that are a command, null ones when using a
    /// NULL string is requested, or when capturing is disabled. When not capturing, the string
    /// representation of the value is injected into the command's text.
    /// </summary>
    protected virtual ICommandInfo.IBuilder Parse(DbTokenValue token)
    {
        // Value is a command...
        if (token.Value is ICommand command)
        {
            var temp = new DbTokenCommand(command);
            return Visit(temp);
        }

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