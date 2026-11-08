#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Builds <see cref="Predicate{T}"/> instances.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
static class PredicateEx
{
    /// <summary>
    /// Returns a predicate that tests for equality with the given value, using the type's own
    /// capabilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0075")]
    public static Predicate<T> ValueEquality<T>(T value) => x =>
        value is IEquatable<T> equatable ? equatable.Equals(x) :
        value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
        false;
}