#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Used to obtain <see cref="IComparer{T}"/> objects. If a custom comparison is provided, then
/// its logic is used. Otherwise, if the type <c>T</c> implements <see cref="IComparable{T}"/>,
/// then the <see cref="Comparer{T}.Default"/> instance is invoked. Finally, the fallback logic
/// is to compare the string representations of the <c>T</c>'s instances.
/// </summary>
public static class ComparerFactory
{
    /// <summary>
    /// Obtains a new instance that implements the <see cref="IEqualityComparer{T}"/> and the
    /// <see cref="IComparer{T}"/> interfaces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static IComparer<T> Create<T>(Comparison<T>? comparison = null)
    {
        if (comparison != null) return Comparer<T>.Create(comparison);

        var type = typeof(T);
        var valid =
            type.IsAssignableFrom(typeof(IComparable<T>)) ||
            type.IsAssignableFrom(typeof(IComparable));
        if (valid) return Comparer<T>.Default;

        return Comparer<T>.Create((x, y) =>
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;
            return string.Compare(x.ToString(), y.ToString());
        });
    }

    // Implements both interfaces.
    class InnerComparer<T> : IEqualityComparer<T>, IComparer<T>
    {
        // Initializes a new instance.
        public InnerComparer(Func<T, T, int>? comparison = null)
        {
            if (comparison != null) _Compare = comparison;
            else
            {
                var type = typeof(T);
                var same = type.IsAssignableFrom(typeof(IComparable<T>)) || type.IsAssignableFrom(typeof(IComparable));
                if (same) _Compare = Comparer<T>.Default.Compare;
                else _Compare = (x, y) =>
                {
                    if (x is null && y is null) return 0;
                    if (x is null) return -1;
                    if (y is null) return +1;
                    return string.Compare(x.ToString(), y.ToString());
                };
            }
        }
        readonly Func<T, T, int> _Compare;

        // Implements IComparer
        public int Compare(T? x, T? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;
            return _Compare(x, y);
        }

        // Implements IEqualityComparer
        public bool Equals(T? x, T? y) => Compare(x, y) == 0;

        // Implements IEqualityComparer
        public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;
    }
}