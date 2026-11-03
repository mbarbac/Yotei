#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class ComparerFactory
{
    /// <summary>
    /// Returns a new instance that implements both the <see cref="IEqualityComparer{T}"/> and
    /// the <see cref="IComparer{T}"/> interfaces, such that:
    /// <br/>- If a custom comparer is provided, then its logic is used.
    /// <br/>- If the '<c>T</c>' type implements <see cref="IComparable{T}"/>, then the default
    /// comparer for that type can be used, and so <see cref="Comparer{T}.Default"/> is used to
    /// implement the comparison logic.
    /// <br/>- If a custom string comparison is provided, the comparison logic is to compare
    /// the string representation of the values using that comparison.
    /// <br/>- If none of the above (in order) can be used, then the comparison fails, and
    /// returns either <c>false</c> or <c>-1</c> by convention.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comparer"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static InnerComparer<T> Create<T>(
        Func<T, T, int>? comparer = null,
        StringComparison? comparison = null) => new(comparer, comparison);

    /// <summary>
    /// Implements both the <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/>
    /// interfaces
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InnerComparer<T> : IEqualityComparer<T>, IComparer<T>
    {
        /// <summary>
        /// Initializes a new instance that compares '<c>T</c>' values, such that:
        /// <br/>- If a custom comparer is provided, then its logic is used.
        /// <br/>- If the '<c>T</c>' type implements <see cref="IComparable{T}"/>, then the default
        /// comparer for that type can be used, and so <see cref="Comparer{T}.Default"/> is used to
        /// implement the comparison logic.
        /// <br/>- If a custom string comparison is provided, the comparison logic is to compare
        /// the string representation of the values using that comparison.
        /// <br/>- If none of the above (in order) can be used, then the comparison fails, and
        /// returns either <c>false</c> or <c>-1</c> by convention.
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="comparison"></param>
        public InnerComparer(
            Func<T, T, int>? comparer = null,
            StringComparison? comparison = null)
        {
            if (comparer != null) _Comparer = comparer;
            else
            {
                var type = typeof(T);
                if (type.IsAssignableFrom(typeof(IComparable<T>)) ||
                    type.IsAssignableFrom(typeof(IComparable)))
                    _Comparer = Comparer<T>.Default.Compare;

                _Comparison = comparison;
                _Comparer = _Comparison is null
                    ? (_, _) => -1
                    : (x, y) =>
                    {
                        if (x is null && y is null) return 0;
                        if (x is null) return -1;
                        if (y is null) return +1;

                        var same = string.Compare(x.ToString(), y.ToString(), _Comparison.Value);
                        if (same > 0) return +1;
                        if (same < 0) return -1;
                        return 0;
                    };

            }
        }
        readonly Func<T, T, int> _Comparer;
        readonly StringComparison? _Comparison;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T? x, T? y) => _Comparer(x!, y!);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T? x, T? y) => Compare(x, y) == 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;
    }
}