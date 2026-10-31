#if YOTEI_TOOLS_GENERATORS
#pragma warning disable CS0436
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class ObjectExtensions
{
    /// <summary>
    /// Throws an exception if the given value is null. Otherwise, returns the value itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(
        [AllowNull, NotNull] this T value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name)) name = nameof(value);

        if (value is null) throw new ArgumentNullException(name);
        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source value can be considered equal to the target one. This
    /// method can be used with reference types, when either <paramref name="source"/> or
    /// <paramref name="target"/> can be <see langword="null"/>. If both are, then equality is
    /// just assumed.
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
    /// Determines if the given source value can be considered equal to the target one, using the
    /// given comparer. This method can be used with reference types, when either
    /// <paramref name="source"/> or <paramref name="target"/> can be <see langword="null"/>. If
    /// both are, then equality is just assumed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsEx<T>(
        [AllowNull] this T source, [AllowNull] T target, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        if (!typeof(T).IsValueType)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;
        }
        return comparer.Equals(source!, target!);
    }
}