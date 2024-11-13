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
}