namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions"/> methods.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// The options to use with type or reflection elements.
    /// </summary>
    public EasyNameOptions EasyOptions
    {
        get => _EasyOptions;
        init => _EasyOptions = value.ThrowWhenNull();
    }
    EasyNameOptions _EasyOptions = EasyNameOptions.Default;

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
    /// Initializes a new default instance with all its properties set to <c>false</c> or
    /// <c>null</c>, except <see cref="NullStr"/> with a value of 'NULL'.
    /// </summary>
    public SketchOptions()
    {
        EasyOptions = EasyNameOptions.False;
        NullStr = "NULL";
        Format = null;
        Provider = null;
        UseShape = true;
        UsePrivateMembers = false;
        UseStaticMembers = false;
    }

    /// <summary>
    /// A shared default instance.
    /// </summary>
    public static SketchOptions Default { get; } = new();

    /// <summary>
    /// A shared instance with its options set to <c>false</c> or <c>null</c>.
    /// </summary>
    public static SketchOptions False { get; } = new()
    {
        EasyOptions = EasyNameOptions.False,
        NullStr = "",
        Format = null,
        Provider = null,
        UseShape = false,
        UsePrivateMembers = false,
        UseStaticMembers = false,
    };

    /// <summary>
    /// A shared instance with its options set to <c>true</c>, except the <see cref="Format"/>
    /// and <see cref="Provider"/> ones, and <see cref="NullStr"/> with a value of 'NULL'.
    /// </summary>
    public static SketchOptions True { get; } = new()
    {
        EasyOptions = EasyNameOptions.True,
        NullStr = "NULL",
        Format = null,
        Provider = null,
        UseShape = true,
        UsePrivateMembers = true,
        UseStaticMembers = true,
    };
}