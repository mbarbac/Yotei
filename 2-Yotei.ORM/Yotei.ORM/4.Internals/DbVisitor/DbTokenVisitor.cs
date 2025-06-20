namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing db-token chains returning the <see cref="ICommandInfo"/>
/// object that represents that chain for the underlying database engine.
/// </summary>
public record class DbTokenVisitor
{
    public const bool USENULLSTRING = true;
    public const bool CAPTUREVALUES = true;
    public const bool CONVERTVALUES = true;
    public const bool USEQUOTES = true;
    public const bool USETERMINATORS = true;
    public const string? RANGESEPARATOR = null;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="locale">If null then using a default with the current culture.</param>
    /// <param name="useNullString"></param>
    /// <param name="captureValues"></param>
    /// <param name="convertValues"></param>
    /// <param name="useQuotes"></param>
    /// <param name="useTerminators"></param>
    /// <param name="rangeSeparator"></param>
    public DbTokenVisitor(
        IConnection connection,
        Locale? locale = null,
        bool useNullString = USENULLSTRING,
        bool captureValues = CAPTUREVALUES,
        bool convertValues = CONVERTVALUES,
        bool useQuotes = USEQUOTES,
        bool useTerminators = USETERMINATORS,
        string? rangeSeparator = RANGESEPARATOR)
    {
        Connection = connection.ThrowWhenNull();

        Locale = locale ?? new();
        UseNullString = useNullString;
        CaptureValues = captureValues;
        ConvertValues = convertValues;
        UseQuotes = useQuotes;
        UseTerminators = useTerminators;
        RangeSeparator = rangeSeparator;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenVisitor(DbTokenVisitor source)
    {
        source.ThrowWhenNull();

        Connection = source.Connection;
        Locale = source.Locale;
        UseNullString = source.UseNullString;
        CaptureValues = source.CaptureValues;
        ConvertValues = source.ConvertValues;
        UseQuotes = source.UseQuotes;
        UseTerminators = source.UseTerminators;
        RangeSeparator = source.RangeSeparator;
    }

    /// <inheritdoc/>
    public override string ToString() => nameof(DbTokenVisitor);

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public IConnection Connection { get; }
    IEngine Engine => Connection.Engine;
    IValueConverterList Converters => Connection.ToDatabaseConverters;

    /// <summary>
    /// The locale to use with culture-sensitive elements.
    /// </summary>
    public Locale Locale { get; init; }

    /// <summary>
    /// Whether to use the engine's null value literal when translating null values, or rather
    /// treat them as regular ones.
    /// </summary>
    public bool UseNullString { get; init; }

    /// <summary>
    /// Whether to capture into parameters the values found while parsing, or not.
    /// </summary>
    public bool CaptureValues { get; init; }

    /// <summary>
    /// Whether to to convert the captured values into database understood ones, or not.
    /// </summary>
    public bool ConvertValues { get; init; }

    /// <summary>
    /// Whether to wrap values not captured between quotes, or rather to emit their raw string
    /// representation.
    /// </summary>
    public bool UseQuotes { get; init; }

    /// <summary>
    /// Whether to use the engine terminators when emitting identifiers, if the engine use them,
    /// or rather to use their raw values.
    /// </summary>
    public bool UseTerminators { get; init; }

    /// <summary>
    /// The separator to use elements in a range, or null if it is not used.
    /// </summary>
    public string? RangeSeparator { get; init; }

    // -----------------------------------------------------

    /// <summary>
    /// Returns the appropriate database representation of the given value.
    /// </summary>
    /// <param name="value"></param>
    public virtual string ToValueString(object? value)
    {
        // Intercepting null values...
        if (value is null)
        {
            return
                UseNullString ? Engine.NullValueLiteral :
                UseQuotes ? "''" : string.Empty;
        }

        // May need to convert values...
        if (ConvertValues)
        {
            value = Converters.TryConvert(value, Locale);
        }

        // Numeric values...
        switch (value)
        {
            case byte item: return item.ToString();
            case sbyte item: return item.ToString();
            case short item: return item.ToString();
            case ushort item: return item.ToString();
            case int item: return item.ToString();
            case uint item: return item.ToString();
            case long item: return item.ToString();
            case ulong item: return item.ToString();

            case bool item: return item.ToString().ToUpper();
            case decimal item: return item.ToString(Locale.CultureInfo);
            case float item: return item.ToString(Locale.CultureInfo);
            case double item: return item.ToString(Locale.CultureInfo);
        }

        // Other values...
        var str = value switch
        {
            char item => item.ToString(Locale.CultureInfo),
            string item => item,

            DateTime item => item.ToString(Locale.CultureInfo),
            DateOnly item => item.ToString(Locale.CultureInfo),
            DayDate item => item.ToString(Locale.CultureInfo),

            TimeOnly item => item.ToString(Locale.CultureInfo),
            ClockTime item => item.ToString(Locale.CultureInfo),
            TimeSpan item => item.ToClockTime().ToString(Locale.CultureInfo),

            _ => value.Sketch()
        };

        // Finishing with quotes if needed...
        return
            str is null ? ToValueString(null) :
            UseQuotes ? $"'{str}'" : str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Visits the chain of tokens obtained from parsing the given dynamic lambda expression,
    /// and returns the command info extracted from it.
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
    /// Visits the chain of tokens represented by the given one as the last node in that chain,
    /// and returns the command info extracted from it.
    /// </summary>
    /// <remarks>This method parses the given token along with its complete hosts' chain, if any,
    /// and its arguments or other contents. It may return a different element if such parsing
    /// renders a different database construct.</remarks>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        token.ThrowWhenNull();

        return token switch
        {
            DbTokenArgument item => VisitToken(item),
            DbTokenBinary item => VisitToken(item),
            DbTokenChain item => VisitToken(item),
            DbTokenCoalesce item => VisitToken(item),
            DbTokenCommand item => VisitToken(item),
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

    /// <summary>
    /// Invoked to visit the given range of tokens, joining the results with the current range
    /// separator in this instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<IDbToken> range)
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
    /// <br/> Arguments are considered translation artifacts, with no representation in database
    /// commands, and so they just parse to empty strings.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenArgument token)
    {
        return new CommandInfo.Builder(Engine);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method intercepts comparison against right-null values, and replaces them by
    /// the appropriate command constructs, as: "IS NULL" and "IS NOT NULL".
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenBinary token)
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
    /// <br/> This method visits the tokens in the given chain joining the results with the
    /// current range separator in this instance.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenChain token)
    {
        return VisitRange(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCoalesce token)
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
    /// <br/> If the command is an empty one, then returns an empty builder. Otherwise, returns
    /// the non-iterable version of the command wrapped with rounded brackets.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCommand token)
    {
        var info = token.Command.GetCommandInfo();

        if (info.IsEmpty)
        {
            return new CommandInfo.Builder(Connection.Engine);
        }
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
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenConvert token)
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
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIdentifier token)
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
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIndexed token)
    {
        var host = Visit(token.Host);
        var temp = this.ToCommaVisitor();
        var args = temp.VisitRange(token.Indexes);

        args.ReplaceText($"[{args.Text}]");
        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method is used to join the representation of its parameters with the one of
    /// its host, without any separators. This provides a way to inject arbitrary contents into
    /// the returned string when such is needed.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenInvoke token)
    {
        var host = Visit(token.Host);
        var temp = this.ToNullSeparatorVisitor();
        var args = temp.VisitRange(token.Arguments);

        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> By convention, contents of literal tokens ARE NOT captured as arguments, and are
    /// just injected into the command's string representation.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenLiteral token)
    {
        return new CommandInfo.Builder(Engine, token.Value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method intercepts virtual method invocations such as 'x => ...x(...)' where
    /// the method name is the same as the name of the dynamic argument, and translates them
    /// into invoke operations to inject their arbitrary contents.
    /// <br/> In addition, it intercepts the following virtual methods translating them into
    /// the appropriate database constructs:
    /// <br/>- Argument-level: NOT, COUNT, CAST, CONVERT.
    /// <br/>- Member-level: AS, IN, NOTIN, BETWEEN, LIKE, NOTLIKE.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenMethod token)
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
                    chain = TryExpand(token.Arguments);
                    temp = this.ToCommaVisitor().VisitRange(chain);
                    host.Add(" IN (");
                    host.Add(temp);
                    host.Add(")");
                    return host;

                case "NOTIN":
                    if (token.Arguments.Count == 0) Throw($"IN(expr, ...) requires at least 1 argument.");
                    chain = TryExpand(token.Arguments);
                    temp = this.ToCommaVisitor().VisitRange(chain);
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
    /// Determines if the given chain is either an empty one, or consists in just one element
    /// whose value is an asterisk.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    protected static bool IsEmptyOrSoleAsterisk(DbTokenChain chain)
    {
        if (chain.Count == 0) return true;

        if (chain.Count == 1 &&
            chain[0] is DbTokenValue value && (
            (value.Value is char c && c == '*') || (value.Value is string s && s == "*")))
            return true;

        return false;
    }

    /// <summary>
    /// Expands the first element of the given chain, provided that it is an enumerable one,
    /// and it is the unique element in that chain. Otherwise, returns the original instance.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    protected DbTokenChain TryExpand(DbTokenChain chain)
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
                        case IDbToken temp:
                            builder.Add(temp);
                            break;

                        case LambdaNode temp:
                            var other = DbLambdaParser.Parse(Engine, temp);
                            builder.Add(other);
                            break;

                        default:
                            builder.Add(new DbTokenValue(item));
                            break;
                    }
                }

                chain = builder.CreateInstance();
            }
        }

        return chain;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method wraps the setter operation between rounded brackets.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenSetter token)
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
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenTernary token)
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
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenUnary token)
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
    /// <br/> This method, by default, captures the given contents as command arguments, unless
    /// these contents are a command itself, null values, or when capturing is disabled in this
    /// instance. In these cases, their string representation is just injected into the command's
    /// text.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenValue token)
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