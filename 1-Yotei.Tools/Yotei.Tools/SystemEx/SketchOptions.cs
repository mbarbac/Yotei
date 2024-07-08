namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions"/> methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// The options to use with the type of the element.
    /// </summary>
    public EasyTypeOptions TypeOptions
    {
        get => _TypeOptions;
        init => _TypeOptions = value.ThrowWhenNull();
    }
    EasyTypeOptions _TypeOptions = default!;

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

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance, with 'NULL' and empty type options.
    /// </summary>
    public SketchOptions()
    {
        TypeOptions = EasyTypeOptions.Empty;
        NullStr = "NULL";
        Format = null;
        Provider = null;
        UseShape = false;
        UsePrivateMembers = false;
        UseStaticMembers = false;
    }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static SketchOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with an empty <see cref="NullStr"/> and empty type options.
    /// </summary>
    public static SketchOptions Empty { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Empty,
        NullStr = "",
        Format = null,
        Provider = null,
        UseShape = false,
        UsePrivateMembers = false,
        UseStaticMembers = false,
    };

    /// <summary>
    /// A common shared instance with all its options set to <c>true</c> or <c>full</c>, except
    /// the <see cref="Format"/> and <see cref="Provider"/> ones.
    /// </summary>
    public static SketchOptions Full { get; } = new()
    {
        TypeOptions = EasyTypeOptions.Full,
        NullStr = "NULL",
        Format = null,
        Provider = null,
        UseShape = true,
        UsePrivateMembers = true,
        UseStaticMembers = true,
    };
}