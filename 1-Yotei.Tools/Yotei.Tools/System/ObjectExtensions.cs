namespace Yotei.Tools;

// ========================================================
public static class ObjectExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> exception when the given value is null.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(
        [AllowNull] this T value,
        [CallerArgumentExpression(nameof(value))] string? description = null)
    {
        description = description.NullWhenEmpty() ?? nameof(description);

        if (value is null) throw new ArgumentNullException(description);
        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given object can be considered equal, intercepting the case where
    /// any of them, or both, are null ones.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsEx<T>(this T x, T y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }
}