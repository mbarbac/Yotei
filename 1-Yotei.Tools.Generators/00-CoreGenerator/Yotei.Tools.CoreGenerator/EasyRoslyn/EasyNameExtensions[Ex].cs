namespace Yotei.Tools.CoreGenerator;

// ========================================================
// Extensions for 'EasyName' purposes only.
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Determines if the type is a generic-alike one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsGenericAlike(
        this ITypeSymbol source) => source.TypeKind == TypeKind.TypeParameter;
}