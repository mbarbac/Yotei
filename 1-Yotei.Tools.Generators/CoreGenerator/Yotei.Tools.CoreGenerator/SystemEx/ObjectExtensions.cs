namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class ObjectExtensions
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
        [AllowNull] this T source,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description)) description = nameof(source);

        if (source is null) throw new ArgumentNullException(description);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Object.Equals(object?, object?)"/>
    /// <br/> If both are '<c>null</c>' references, returns '<c>true</c>' instead of throwing any
    /// exceptions. If only one is '<c>null</c>', then returns '<c>false</c>'. Otherwise, invokes
    /// the source's 'Equals' method with the target instance as its argument.
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
}