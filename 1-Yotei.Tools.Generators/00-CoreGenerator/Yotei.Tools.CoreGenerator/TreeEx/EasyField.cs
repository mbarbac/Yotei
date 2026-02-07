namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyField
{
    /// <summary>
    /// Include the accessibility modifiers of the member (ie: public).
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// Include the modifiers of the member (ie: static readonly).
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to include the type of the property. If null, it is ignored.
    /// </summary>
    public EasyType? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyType? HostTypeOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default settings.
    /// </summary>
    public static EasyField Default => new()
    {
        ReturnTypeOptions = EasyType.Default,
        HostTypeOptions = EasyType.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyField Full => new()
    {
        UseAccessibility = true,
        UseModifiers = true,
        ReturnTypeOptions = EasyType.Full,
        HostTypeOptions = EasyType.Full,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{/// <summary>
 /// Gets a new format instance suitable for EasyName purposes.
 /// </summary>
    static SymbolDisplayFormat ToDisplayFormat(EasyField options)
    {
        return new SymbolDisplayFormat();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IFieldSymbol source, EasyField options)
    {
        throw null;
    }
}