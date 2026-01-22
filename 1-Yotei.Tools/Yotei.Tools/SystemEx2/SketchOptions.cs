namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'Sketch' methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// Determines if the type of the element shall be used as a prefix of the obtained alternate
    /// string representation. If not, then this type is ignored.
    /// </summary>
    public bool UseTypeHead { get; init; }

    /// <summary>
    /// If true, all suitable 'ToString' methods are ignored.
    /// </summary>
    public bool PreventToString { get; init; }

    /// <summary>
    /// If not null, the format string used when formatting the value, if a suitable 'ToString'
    /// method exist.
    /// </summary>
    public string? FormatString { get; init; }

    /// <summary>
    /// If not null, the format provider used when formatting the value, if a suitable 'ToString'
    /// method exist.
    /// </summary>
    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// If not null, the literal used to represent NULL values. If null, then an empty string
    /// is used.
    /// </summary>
    public string? NullString { get; init; }

    /// <summary>
    /// If not null, then the options to use with 'Type' and reflection-alike elements. If null,
    /// then a set of default settings is used.
    /// </summary>
    public EasyNameOptions? EasyNameOptions { get; init; }

    /// <summary>
    /// If true, obtains the shape of the element if other possible routes have not succeeded.
    /// The shape is built from the public members of the element.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// If true, include in the shape the private members of the element.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// If true, include in the shape the static members of the element.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// A shared instance with empty settings.
    /// </summary>
    public static SketchOptions Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static SketchOptions Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static SketchOptions Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() : this(Mode.Empty) { }
    enum Mode { Empty, Default, Full };
    SketchOptions(Mode mode)
    {
        switch (mode)
        {
            case Mode.Full:
                UseTypeHead = true;
                NullString = "NULL";
                EasyNameOptions = EasyNameOptions.Full;
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;

            default:
            case Mode.Default:
                NullString = "NULL";
                EasyNameOptions = EasyNameOptions.Default;
                UseShape = true;
                break;

            case Mode.Empty:
                break;
        }
    }
}