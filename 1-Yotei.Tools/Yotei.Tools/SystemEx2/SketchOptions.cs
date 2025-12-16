namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 'Sketch(...)' family of methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static SketchOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static SketchOptions Default => new(BuildMode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static SketchOptions Full => new(BuildMode.Full);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public SketchOptions() : this(BuildMode.Default) { }

    enum BuildMode { Empty, Default, Full }
    private SketchOptions(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Empty:
                break;

            case BuildMode.Default:
                NullString = "NULL";
                TypeOptions = EasyNameOptions.Default;
                MemberInfoOptions = EasyNameOptions.Default;
                UseShape = true;
                break;

            case BuildMode.Full:
                NullString = "NULL";
                TypeOptions = EasyNameOptions.Full;
                MemberInfoOptions = EasyNameOptions.Full;
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;
        }
    }

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
    /// If not null, then the options to use with the type of the source value.
    /// </summary>
    public EasyNameOptions? TypeOptions { get; init; }

    /// <summary>
    /// If not null, then the options to use when the value is a reflection-alike one.
    /// </summary>
    public EasyNameOptions? MemberInfoOptions { get; init; }

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
}