namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class NullableHelpers
{
    /// <summary>
    /// Tries to obtain the underlying type from the given one, provided that it is a nullable
    /// one, or returns the original one otherwise.
    /// <br/> This method essentially returns the type argument of the 'Nullable{T}' or the
    /// 'IsNullable{T}' types, or the original type instead. In addition, sets the out parameter
    /// with the appropriate value.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isnullable"></param>
    /// <returns></returns>
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