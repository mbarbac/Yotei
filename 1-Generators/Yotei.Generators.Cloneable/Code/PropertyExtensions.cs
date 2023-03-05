namespace Yotei.Generators.Cloneable;

// ========================================================
internal static class PropertyExtensions
{
    /// <summary>
    /// Determines if this symbol is market to be ignored while cloning, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsIgnore(this IPropertySymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        if (symbol.HasAttribute(CloneableMemberSource.AttributeLongName, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberSource.Ignore);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if this symbol is market for deep cloning, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsDeep(this IPropertySymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        if (symbol.HasAttribute(CloneableMemberSource.AttributeLongName, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberSource.Deep);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }
        return false;
    }
}