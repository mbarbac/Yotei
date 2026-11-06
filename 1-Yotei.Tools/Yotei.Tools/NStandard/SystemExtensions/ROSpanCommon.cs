#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

// Cannot use extension blocks because any restrictions on 'T' at that level apply to all methods,
// and we have methods with different restrictions.
namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class ReadOnlySpanExtensions
{
    /// <summary>
    /// Returns a predicate that tests for equality against the given value.
    /// </summary>
    static Predicate<T> InnerEquality<T>(T value) => x =>
        value is IEquatable<T> equatable ? equatable.Equals(x) :
        value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
        false;

    // ----------------------------------------------------

    /// <summary>
    /// Used to instantiate an appropriate comparer that either uses the given one, or tries to
    /// use the type's own capabilities.
    /// </summary>
    readonly struct InnerComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        readonly IComparer<T>? _PureComparer;
        readonly bool _ThrowTypeException;

        /// <summary>
        /// Initializes a new instance that only uses the type's capabilities.
        /// </summary>
        public InnerComparer(bool throwTypeException = false)
        {
            _PureComparer = null;
            _ThrowTypeException = false;
        }

        /// <summary>
        /// Initializes a new instance that uses the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public InnerComparer(IComparer<T> comparer, bool throwTypeException = false)
        {
            _PureComparer = comparer.ThrowWhenNull();
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            // A comparer was given...
            if (_PureComparer is not null) return _PureComparer.Compare(x, y);
            
            // Comparisons against null...
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;

            // Implements IComparable<T>...
            var type = typeof(T);
            if (typeof(IComparable<T>).IsAssignableFrom(type))
            {
                var data = ((IComparable<T>)x).CompareTo(y);
                if (data < 0) return -1;
                if (data > 0) return +1;
                return 0;
            }

            // Implements IEquatable<T>...
            if (typeof(IEquatable<T>).IsAssignableFrom(type))
            {
                var data = ((IEquatable<T>)x).Equals(y);
                if (data) return 0;
            }

            // T is invalid...
            throw new TypeException(
                "Type does not implement IComparable<T>, and instances are not equal.")
                .WithData(type)
                .WithData(x)
                .WithData(y);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Equals(T? x, T? y)
        {
            // A comparer was given...
            if (_PureComparer is not null)
            {
                var data = _PureComparer.Compare(x, y);
                return data == 0;
            }

            // Comparisons against null...
            if (x is null && y is null) return true;
            if (x is null) return false;
            if (y is null) return false;

            // Implements IEquatable<T>...
            var type = typeof(T);
            if (typeof(IEquatable<T>).IsAssignableFrom(type)) return ((IEquatable<T>)x).Equals(y);

            // Implements IComparable<T>...
            if (typeof(IComparable<T>).IsAssignableFrom(type))
            {
                var data = ((IComparable<T>)x).CompareTo(y);
                return data == 0;
            }

            // T is invalid...
            if (_ThrowTypeException) throw new TypeException(
                "Type does not implement IComparable<T> nor IEquatable<T>.")
                .WithData(type)
                .WithData(x)
                .WithData(y);

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;
    }
}