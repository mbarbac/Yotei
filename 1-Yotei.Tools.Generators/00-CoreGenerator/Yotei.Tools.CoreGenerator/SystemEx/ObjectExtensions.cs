namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ObjectExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when the given source value is null.
    /// Otherwise, returns that value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(
        [AllowNull, NotNull] this T source,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description)) description = nameof(source);

        if (source is null) throw new ArgumentNullException(description);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines whether the given object instances are considered equal. If both are 'null' ones,
    /// returns 'true'. If only one is 'null', returns 'false'. Otherwise, invokes the 'Equal' method
    /// of the source instance with the target one as its argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsEx<T>([AllowNull] this T source, [AllowNull] T target)
    {
        if (!typeof(T).IsValueType)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;
        }
        return source!.Equals(target);
    }

    /// <summary>
    /// Determines whether the given object instances are considered equal. If both are 'null' ones,
    /// returns 'true'. If only one is 'null', returns 'false'. Otherwise, invokes the given comparer
    /// with the source and target instances as its arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EqualsEx<T>(
       [AllowNull] this T source, [AllowNull] T target, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        if (!typeof(T).IsValueType)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;
        }
        return comparer.Equals(source!, target!);
    }
}