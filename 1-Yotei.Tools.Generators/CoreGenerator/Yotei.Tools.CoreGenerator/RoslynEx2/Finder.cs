namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class FinderExtensions
{
    /// <summary>
    /// The delegate used to find a value of the requested type using the given type. Returns
    /// '<c>true</c>' if the value was found, and that value in the out argument. Otherwise,
    /// return '<c>false</c>'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool Delegate<T>(INamedTypeSymbol type, out T value);
}