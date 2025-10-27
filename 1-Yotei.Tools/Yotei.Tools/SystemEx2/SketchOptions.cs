namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'Sketch(...)' family of methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// A shared instance with all settings set to false or null.
    /// </summary>
    public static SketchOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// A shared instance with default useful settings.
    /// </summary>
    public static SketchOptions Default => new(BuildMode.Default);

    /// <summary>
    /// A shared instance with full settings enabled.
    /// </summary>
    public static SketchOptions Full => new(BuildMode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public SketchOptions() : this(BuildMode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the format string used when formatting the value.
    /// </summary>
    public string? FormatString { get; init => field = value?.NotNullNotEmpty(true); }

    /// <summary>
    /// If not null, then the format provider used when formatting the value.
    /// </summary>
    public IFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// If not null, then the string to use with null values.
    /// </summary>
    public string? NullString { get; init; }

    /// <summary>
    /// If not null, then the options to use when the value is a reflection-alike one.
    /// </summary>
    public EasyNameOptions? ReflectionOptions { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// If not null, then the options to use with the type of the source value.
    /// </summary>
    public EasyNameOptions? SourceTypeOptions { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// If true, then use the value shape, using the value members.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// When using the value shape, if true also include its private members.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// When using the value shape, if true also include its static members.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // ----------------------------------------------------

    enum BuildMode { Empty, Default, Full };

    private SketchOptions(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Empty:
                break;

            case BuildMode.Default:
                NullString = "NULL";
                ReflectionOptions = EasyNameOptions.Default;
                UseShape = true;
                break;

            default:
            case BuildMode.Full:
                NullString = "NULL";
                ReflectionOptions = EasyNameOptions.Full;
                SourceTypeOptions = EasyNameOptions.Full;
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;
        }
    }
}