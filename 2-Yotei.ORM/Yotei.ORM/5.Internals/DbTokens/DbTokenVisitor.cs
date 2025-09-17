namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing db-token chains returning the <see cref="ICommandInfo"/>
/// object that represents that chain for the underlying database engine.
/// </summary>
public partial record class DbTokenVisitor
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
    /// <param name="locale"></param>
    public DbTokenVisitor(IConnection connection, Locale locale)
    {
        Connection = connection.ThrowWhenNull();
        Locale = locale.ThrowWhenNull();
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
    /// Returns a clone of this instance but with all its properties set to false or null, except
    /// its <see cref="Locale"/> one, which is kept with its previous value.
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

    /// <summary>
    /// Returns a clone of this instance but with the range separator set to null.
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToNullSeparatorVisitor() => RangeSeparator is null
        ? this
        : this with { RangeSeparator = null };

    /// <summary>
    /// Returns a clone of this instance but with the range separator set to ", ".
    /// </summary>
    /// <returns></returns>
    public DbTokenVisitor ToCommaVisitor() => RangeSeparator is ", "
        ? this
        : this with { RangeSeparator = ", " };

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public IConnection Connection { get; }
    IEngine Engine => Connection.Engine;
    IValueConverterList Converters => Connection.ValueConverters;
    StringComparison Comparison => Engine.CaseSensitiveNames
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// The locale to use with culture-sensitive elements.
    /// </summary>
    public Locale Locale { get; init; }

    /// <summary>
    /// Whether to use the engine's null value literal when translating null values, or rather
    /// treat them as regular ones.
    /// </summary>
    public bool UseNullString { get; init; } = USENULLSTRING;

    /// <summary>
    /// Whether to capture into parameters the values found while parsing, or not.
    /// </summary>
    public bool CaptureValues { get; init; } = CAPTUREVALUES;

    /// <summary>
    /// Whether to to convert the captured values into database understood ones, or not.
    /// </summary>
    public bool ConvertValues { get; init; } = CONVERTVALUES;

    /// <summary>
    /// Whether to wrap values not captured between quotes, or rather to emit their raw string
    /// representation.
    /// </summary>
    public bool UseQuotes { get; init; } = USEQUOTES;

    /// <summary>
    /// Whether to use the engine terminators when emitting identifiers, if the engine use them,
    /// or rather to use their raw values.
    /// </summary>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    /// <summary>
    /// The separator to use elements in a range, or null if it is not used.
    /// </summary>
    public string? RangeSeparator { get; init; } = RANGESEPARATOR;

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
                UseQuotes ? "''" :
                string.Empty;
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
}