namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyNameOptions
{
    /// <summary>
    /// A default shared instance.
    /// </summary>
    public static EasyNameOptions Default { get; } = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public EasyNameOptions() { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (PreventGenericTypeNames) Add(nameof(PreventGenericTypeNames));
        if (UseNameSpace) Add(nameof(UseNameSpace));
        if (UseFullTypeName) Add(nameof(UseFullTypeName));
        if (UseTypeName) Add(nameof(UseTypeName));
        if (PreventReturnType) Add(nameof(PreventReturnType));
        if (PreventArguments) Add(nameof(PreventArguments));

        if (sb.Length == 0) sb.Append('-');
        return sb.ToString();

        void Add(string str) { if (sb.Length > 0) sb.Append(", "); sb.Append(str); }
    }

    /// <summary>
    /// Whether to prevent the usage of generic names, leaving them blank, or to use their actual
    /// names instead.
    /// </summary>
    public bool PreventGenericTypeNames { get; init; } = false;

    /// <summary>
    /// Whether to use the namespace before the type name, or not. Using this option implies using
    /// the type's full name.
    /// </summary>
    public bool UseNameSpace { get; init; } = false;

    /// <summary>
    /// Whether to use the host type name, or not.
    /// </summary>
    public bool UseTypeName { get; init; } = false;

    /// <summary>
    /// Whether to use the type full name, or not.
    /// </summary>
    public bool UseFullTypeName { get; init; } = false;

    /// <summary>
    /// For non-type elements, whether to prevent the element's return type, if any.
    /// </summary>
    public bool PreventReturnType { get; init; } = false;

    /// <summary>
    /// For non-type elements, wheter to prevent the usage or arguments, or not.
    /// </summary>
    public bool PreventArguments { get; init; } = false;
}