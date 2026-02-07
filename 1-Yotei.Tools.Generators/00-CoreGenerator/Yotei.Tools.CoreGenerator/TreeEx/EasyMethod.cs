namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyMethod
{
    /// <summary>
    /// Include the modifiers of the member (ie: static readonly).
    /// </summary>
    public bool UseModifiers { get; set; }

    /// <summary>
    /// Include the accessibility modifiers of the member (ie: public).
    /// </summary>
    public bool UseAccessibility { get; set; }

    /// <summary>
    /// If not null, the options to include the return type of the member. If null, it is ignored.
    /// </summary>
    public EasyType? ReturnTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the host type of the member. If null, it is ignored.
    /// </summary>
    public EasyType? HostTypeOptions { get; set; }

    /// <summary>
    /// If not null, the options to include the generic arguments of the member, if any. If null,
    /// they are ignored.
    /// </summary>
    public EasyType? GenericOptions { get; set; }

    /// <summary>
    /// If enabled, include member brackets, even if <see cref="ParameterOptions"/> is null.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to include the parameters of the member. If null, they are ignored.
    /// </summary>
    public EasyParameter? ParameterOptions { get; set; }
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source) => source.EasyName(new());

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this IMethodSymbol source, EasyMethod options)
    {
        throw null;
    }
}