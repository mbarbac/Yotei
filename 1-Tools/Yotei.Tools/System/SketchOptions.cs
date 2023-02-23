namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="SketchExtensions.Sketch{T}(T?, SketchOptions)"/>
/// extension method.
/// </summary>
public record SketchOptions
{
    /// <summary>
    /// A default shared instance.
    /// </summary>
    public static SketchOptions Default { get; } = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public SketchOptions() { }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString()
    {
        var sb = new StringBuilder();

        Add($"Null={NullStr}");
        if (Format != null) Add($"Format:'{Format}'");
        if (Provider != null) Add($"Provider:{Provider.GetType().Name}");
        if (PreventGenericTypeNames) Add(nameof(PreventGenericTypeNames));
        if (UseNameSpace) Add(nameof(UseNameSpace));
        if (UseTypeName) Add(nameof(UseTypeName));
        if (UseFullTypeName) Add(nameof(UseFullTypeName));
        if (PreventReturnType) Add(nameof(PreventReturnType));
        if (PreventArguments) Add(nameof(PreventArguments));
        if (PreventShape) Add(nameof(PreventShape));
        if (UsePrivateMembers) Add(nameof(UsePrivateMembers));
        if (UseStaticMembers) Add(nameof(UseStaticMembers));

        if (sb.Length == 0) sb.Append('-');
        return sb.ToString();

        void Add(string str) { if (sb.Length > 0) sb.Append(", "); sb.Append(str); }
    }

    /// <summary>
    /// The string to use to represent null values.
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
    public string? Format { get; init; } = null;

    /// <summary>
    /// If not null, the format provider to use.
    /// </summary>
    public IFormatProvider? Provider { get; init; } = null;

    /// <summary>
    /// Whether to prevent the usage of generic names, leaving them blank, or to use their actual
    /// names instead.
    /// </summary>
    public bool PreventGenericTypeNames { get; init; } = false;

    /// <summary>
    /// Whether to use the namespace before the type name, or not. Note that using this option
    /// implies using the type's full name.
    /// </summary>
    public bool UseNameSpace { get; init; } = false;

    /// <summary>
    /// For other elements but types, whether to use the declaring type or not.
    /// </summary>
    public bool UseTypeName { get; init; } = false;

    /// <summary>
    /// Whether to use the type full name, or not.
    /// </summary>
    public bool UseFullTypeName { get; init; } = false;

    /// <summary>
    /// Whether to prevent the usage of the element's return type, if any.
    /// </summary>
    public bool PreventReturnType { get; init; } = false;

    /// <summary>
    /// Wheter to prevent the usage or arguments, or not.
    /// </summary>
    public bool PreventArguments { get; init; } = false;

    /// <summary>
    /// If true prevents the usage of the source's shape, if such would be needed.
    /// </summary>
    public bool PreventShape { get; init; } = false;

    /// <summary>
    /// If true enforces including private members for objects' shapes.
    /// </summary>
    public bool UsePrivateMembers { get; init; } = false;

    /// <summary>
    /// If true enforces including static members for objects' shapes.
    /// </summary>
    public bool UseStaticMembers { get; init; } = false;

    /// <summary>
    /// Returns a new EasyNameOptions instance based upon the contents of this one.
    /// </summary>
    public EasyNameOptions EasyNameOptions => new EasyNameOptions() with {
        PreventGenericTypeNames = PreventGenericTypeNames,
        UseNameSpace = UseNameSpace,
        UseTypeName = UseTypeName,
        UseFullTypeName = UseFullTypeName,
        PreventReturnType = PreventReturnType,
        PreventArguments = PreventArguments
    };
}