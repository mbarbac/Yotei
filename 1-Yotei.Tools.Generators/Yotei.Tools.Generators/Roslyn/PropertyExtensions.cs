namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class PropertyExtensions
{
    /// <summary>
    /// Determines if the property has a suitable getter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasGetter(this IPropertySymbol symbol)
    {
        return symbol.GetMethod != null;
    }

    /// <summary>
    /// Determines if the property has a suitable setter or initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasSetterOrInit(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null;
    }

    /// <summary>
    /// Determines if the property has a suitable setter that is not an initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasSetterOnly(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null && !symbol.SetMethod.IsInitOnly;
    }

    /// <summary>
    /// Determines if the property has a suitable initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasInitOnly(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null && symbol.SetMethod.IsInitOnly;
    }
}