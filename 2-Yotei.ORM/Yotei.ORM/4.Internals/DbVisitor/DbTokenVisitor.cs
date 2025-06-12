namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing db-token chains returns the <see cref="ICommandInfo"/>
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
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        return token.ThrowWhenNull() switch
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
    /// Returns a clone of this instance with the range separator set to ", ".
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToCommaVisitor() => this with { RangeSeparator = ", " };

    /// <summary>
    /// Returns a clone of this instance with the range separator set null.
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToNullSeparatorVisitor() => this with { RangeSeparator = null };

    /// <summary>
    /// Returns a clone of this instance with all its properties set to <c>false</c> or to
    /// <c>null</c>.
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
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenConvert token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIdentifier token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIndexed token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenInvoke token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenLiteral token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenMethod token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenSetter token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenTernary token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenUnary token) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenValue token) => throw null;
}