namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class NullableHelpers
{
    /// <summary>
    /// Obtains the underlying type from the given one, when it is wrapped as a nullable one,
    /// or the original type otherwise.
    /// </summary>
    public static INamedTypeSymbol UnwrapNullable(this INamedTypeSymbol type, out bool isnullable)
    {
        type.ThrowWhenNull();

        if (type.TypeArguments.Length == 1 && (
            type.Name == "IsNullable" || type.Name == "Nullable"))
        {
            type = (INamedTypeSymbol)type.TypeArguments[0];
            isnullable = true;
            return type;
        }

        isnullable = false;
        return type;
    }
}