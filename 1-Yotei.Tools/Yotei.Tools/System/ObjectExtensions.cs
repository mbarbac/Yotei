namespace Yotei.Tools;

// ========================================================
public static class ObjectExtensions
{
    /// <summary>
    /// Determines if the two given object can be considered equal, intercepting the case when
    /// any of them, or both, are null ones.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool EqualsEx<T>(this T x, T y) =>
        (x is null && y is null) ||
        (x is not null && x.Equals(y));
}