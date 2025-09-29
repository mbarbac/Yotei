namespace Yotei.Tools;

// =============================================================
public static class ObjectExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when the given value is null. Otherise,
    /// returns the given value.
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
        description = description.NullWhenEmpty(true) ?? nameof(description);

        if (value is null) throw new ArgumentNullException(description);
        return value;
    }

    // ---------------------------------------------------------

    /// <summary>
    /// Determines if the two given object can be considered equal or not. If both are null, then
    /// this method returns '<c>true</c>'. If only one of them is null, then this method returns
    /// '<c>false</c>'. Otherwise, this method returns the result of invoking the 'Equals' method
    /// on the left instance, with the right one as its parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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