namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions"/> methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// The options to use with not-null reflection elements.
    /// </summary>
    public EasyNameOptions EasyNameOptions
    {
        get => _EasyNameOptions;
        init => _EasyNameOptions = value.ThrowWhenNull();
    }
    EasyNameOptions _EasyNameOptions = null!;

    /// <summary>
    /// The string used to represent null values.
    /// </summary>
    public string NullStr
    {
        get => _NullStr;
        init => _NullStr = value.ThrowWhenNull();
    }
    string _NullStr = default!;

    /// <summary>
    /// If not null, the options to use to preceed values with their types.
    /// </summary>
    public EasyNameOptions? UseSourceType { get; init; }

    /// <summary>
    /// If not null, the format specification to use.
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// If not null, the format provider to use.
    /// </summary>
    public IFormatProvider? Provider { get; init; }

    /// <summary>
    /// If true uses the source's shape, if such would be needed.
    /// </summary>
    public bool UseShape { get; init; }

    /// <summary>
    /// If true enforces including private members for objects' shapes.
    /// </summary>
    public bool UsePrivateMembers { get; init; }

    /// <summary>
    /// If true enforces including static members for objects' shapes.
    /// </summary>
    public bool UseStaticMembers { get; init; }

    // -----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions()
    {
        EasyNameOptions = EasyNameOptions.Empty;
        NullStr = string.Empty;
        UseSourceType = null;
        Format = null;
        Provider = null;
        UseShape = false;
        UsePrivateMembers = false;
        UseStaticMembers = false;
    }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SketchOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static SketchOptions Default => _Default ??= new SketchOptions
    {
        EasyNameOptions = EasyNameOptions.Default,
        NullStr = "NULL",
        UseShape = true,
    };
    static SketchOptions _Default = null!;

    /// <summary>
    /// A shared instance with full settings (except the <see cref="Format"/> and
    /// <see cref="Provider"/> ones).
    /// </summary>
    public static SketchOptions Full => _Full ??= new SketchOptions
    {
        EasyNameOptions = EasyNameOptions.Full,
        NullStr = "NULL",
        UseSourceType = EasyNameOptions.Full,
        UseShape = true,
        UsePrivateMembers = true,
        UseStaticMembers = true,
    };
    static SketchOptions _Full = null!;
}