namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options to use by the '<c>Sketch(...)</c>' family of methods.
/// </summary>
public record SketchOptions
{
    enum BuildMode { Default, Empty, Full };

    private SketchOptions(BuildMode mode)
    {
        Format = null;
        Provider = null;

        switch (mode)
        {
            // All false or null...
            case BuildMode.Empty:
                NullString = string.Empty;
                ReflectionOptions = null;
                SourceTypeOptions = null;
                UseShape = false;
                UsePrivateMembers = false;
                UseStaticMembers = false;
                break;

            // Use NullString, UseShape, and default easy names...
            case BuildMode.Default:
                NullString = "NULL";
                ReflectionOptions = EasyNameOptions.Default;
                SourceTypeOptions = null;
                UseShape = true;
                UsePrivateMembers = false;
                UseStaticMembers = false;
                break;

            // All true or full...
            default:
                NullString = "NULL";
                ReflectionOptions = EasyNameOptions.Full;
                SourceTypeOptions = EasyNameOptions.Full;
                UseShape = true;
                UsePrivateMembers = true;
                UseStaticMembers = true;
                break;
        }
    }

    /// <summary>
    /// An instance with common useful settings.
    /// </summary>
    public static SketchOptions Empty => new(BuildMode.Empty);

    /// <summary>
    /// An instance with common useful settings.
    /// </summary>
    public static SketchOptions Default => new(BuildMode.Default);

    /// <summary>
    /// An instance with common useful settings.
    /// </summary>
    public static SketchOptions Full => new(BuildMode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public SketchOptions() : this(BuildMode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, then the format string to use when formatting the value.
    /// </summary>
    public string? Format { get; init => field = value?.NotNullNotEmpty(true); }

    /// <summary>
    /// If not null, then the format provider to use when formatting the value.
    /// </summary>
    public IFormatProvider? Provider { get; init; }

    /// <summary>
    /// The string to use to represent null values.
    /// </summary>
    public string NullString { get => field; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// If not null, then the options to use with reflection-alike elements, such as types and
    /// meber info instances.
    /// </summary>
    public EasyNameOptions? ReflectionOptions { get; init => field = value?.ThrowWhenNull(); }

    /// <summary>
    /// If not null, then the options to use to preceed the value with its type.
    /// </summary>
    public EasyNameOptions? SourceTypeOptions { get; init => field = value?.ThrowWhenNull(); }

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