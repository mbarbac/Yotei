namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyParameter
{
    /// <summary>
    /// Include the 'this' keyword before the first parameter of an extension method, in C#.
    /// </summary>
    public bool UseThis { get; set; }

    /// <summary>
    /// Include the 'params', 'scoped', 'ref', 'in', 'out', 'ByRef', and 'ByVal' keywords before
    /// the parameter, if possible.
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// If not null, the options to include the parameter's type. If null, it is ignored.
    /// </summary>
    public EasyType? TypeOptions { get; set; }

    /// <summary>
    /// Include the name of the parameter.
    /// </summary>
    public bool UseName { get; set; }

    /// <summary>
    /// Include the default value of the parameter, if any. This setting is only used it the
    /// <see cref="UseName"/> one is also enabled.
    /// </summary>
    public bool UseDefaultValue { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default settings.
    /// </summary>
    public static EasyParameter Default => new()
    {
        UseModifiers = true,
        TypeOptions = EasyType.Default,
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyParameter Full => new()
    {
        UseThis = true,
        UseModifiers = true,
        TypeOptions = EasyType.Full,
        UseName = true,
        UseDefaultValue = true,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Gets a new format instance suitable for EasyName purposes.
    /// </summary>
    static SymbolDisplayFormat ToDisplayFormat(EasyParameter options)
    {
        var pars = default(SymbolDisplayParameterOptions);
        if (options.UseThis) pars |= SymbolDisplayParameterOptions.IncludeExtensionThis;
        if (options.UseModifiers) pars |= SymbolDisplayParameterOptions.IncludeModifiers;

        return new SymbolDisplayFormat();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IParameterSymbol source, EasyParameter options)
    {
        var format = ToDisplayFormat(options);
        var name = source.ToDisplayString(format);
        return name;
    }
}