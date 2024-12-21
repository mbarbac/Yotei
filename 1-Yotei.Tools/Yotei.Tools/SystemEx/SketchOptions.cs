namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions"/> methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SketchOptions() { }

    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SketchOptions Empty { get; } = new();

    /// <summary>
    /// A shared instance with default settings.
    /// </summary>
    public static SketchOptions Default
    {
        get
        {
            if (_Default == null)
            {
                _Default = new SketchOptions
                {
                    ReflectionOptions = EasyNameOptions.Default,
                    NullStr = "NULL",
                    UseShape = true,
                };
            }
            return _Default;
        }
    }
    static SketchOptions _Default = default!;

    /// <summary>
    /// A shared instance with full settings.
    /// </summary>
    public static SketchOptions Full
    {
        get
        {
            if (_Full == null)
            {
                _Full = new SketchOptions
                {
                    ReflectionOptions = EasyNameOptions.Full,
                    NullStr = "NULL",
                    UseSourceType = EasyNameOptions.Full,
                    UseShape = true,
                    UsePrivateMembers = true,
                    UseStaticMembers = true,
                };

            }
            return _Full;
        }
    }
    static SketchOptions _Full = default!;

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the format specification to use.
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// If not null, the format provider to use.
    /// </summary>
    public IFormatProvider? Provider { get; init; }

    /// <summary>
    /// The options to use with reflection-alike elements.
    /// </summary>
    public EasyNameOptions ReflectionOptions
    {
        get => _ReflectionOptions;
        init => _ReflectionOptions = value.ThrowWhenNull();
    }
    EasyNameOptions _ReflectionOptions = EasyNameOptions.Empty;

    /// <summary>
    /// The string used to represent null values.
    /// </summary>
    public string NullStr
    {
        get => _NullStr;
        init => _NullStr = value.ThrowWhenNull();
    }
    string _NullStr = string.Empty;

    /// <summary>
    /// If not null, the options used to preceed the values with their types.
    /// </summary>
    public EasyNameOptions? UseSourceType { get; init; }

    /// <summary>
    /// If true use the source value shape, meaning the values of its public members at least,
    /// if such is needed.
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
}
