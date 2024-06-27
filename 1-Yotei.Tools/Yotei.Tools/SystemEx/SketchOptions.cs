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
    EasyTypeOptions _TypeOptions = new EasyTypeOptions { UseName = false };

    /// <summary>
    /// The string used to represent null values.
    /// </summary>
    public string NullStr
    {
        get => _NullStr;
        init => _NullStr = value.NotNullNotEmpty();
    }
    string _NullStr = "NULL";

    /// <summary>
    /// If not null, the format specification to use.
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// If not null, the format provider to use.
    /// </summary>
    public IFormatProvider? Provider { get; init; }

    /// <summary>
    /// If true prevents the usage of the source's shape, if such would be needed.
    /// </summary>
    public bool PreventShape { get; init; }

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
    /// Initializes a new default instance.
    /// </summary>
    public SketchOptions() { }

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static SketchOptions Default { get; } = new();
}