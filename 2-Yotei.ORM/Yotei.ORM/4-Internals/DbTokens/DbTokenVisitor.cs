namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing <see cref="IDbToken"/> chains returning the appropriate
/// database representation.
/// </summary>
public partial record DbTokenVisitor
{
    public const bool USENULLSTRING = true;
    public const bool CAPTUREVALUES = true;
    public const bool CONVERTVALUES = true;
    public const bool USEQUOTES = true;
    public const bool USETERMINATORS = true;
    public const string? RANGESEPARATOR = ", ";

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
    /// <param name="other"></param>
    protected DbTokenVisitor(DbTokenVisitor other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Connection = other.Connection;
        Locale = other.Locale;
        UseNullString = other.UseNullString;
        CaptureValues = other.CaptureValues;
        ConvertValues = other.ConvertValues;
        UseQuotes = other.UseQuotes;
        UseTerminators = other.UseTerminators;
        RangeSeparator = other.RangeSeparator;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public IConnection Connection { get; }
    IEngine Engine => Connection.Engine;
    IValueConverterList Converters => Connection.ToDbConverters;

    /// <summary>
    /// The locale to use with culture-sensitive elements.
    /// </summary>
    public Locale Locale { get; init; }

    /// <summary>
    /// Determines if the engine's <see cref="IEngine.NullValueLiteral"/> shall be used with
    /// null values, or rather if they are trated as regular ones.
    /// </summary>
    public bool UseNullString { get; init; } = USENULLSTRING;

    /// <summary>
    /// Determines if the values found while parsing shall be captured as arguments, or rather
    /// injected into the command's text.
    /// </summary>
    public bool CaptureValues { get; init; } = CAPTUREVALUES;

    /// <summary>
    /// Determines if the <see cref="IConnection.ToDbConverters"/> collection shall be used to
    /// converte application-level values to database ones, or not.
    /// </summary>
    public bool ConvertValues { get; init; } = CONVERTVALUES;

    /// <summary>
    /// Determines if the injected values shall be wrapped between quotes, or not.
    /// </summary>
    public bool UseQuotes { get; init; } = USEQUOTES;

    /// <summary>
    /// Determines if the engine's terminators shall be used to wrapp any identifiers found, if
    /// the engine uses them, or not.
    /// </summary>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    /// <summary>
    /// Determines the separator to use with the elements of a range, provided it is not a null
    /// one. If null, then it is ignored.
    /// </summary>
    public string? RangeSeparator { get; init; } = RANGESEPARATOR;

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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate database representation of the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual string ToValueString(object? value)
    {
        // Intercepting null values...
        if (value is null)
        {
            return UseNullString
                ? Engine.NullValueLiteral
                : (UseQuotes ? "''" : string.Empty);
        }

        // May need to convert the value...
        if (ConvertValues) value = Converters.TryConvert(value, Locale);

        // Standard cases...
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
            TimeSpan item => ((ClockTime)item).ToString(Locale.CultureInfo),

            _ => value.Sketch()
        };

        // Finishing with quotes if needed...
        return
            str is null
            ? ToValueString(null)
            : (UseQuotes ? $"'{str}'" : str);
    }
}