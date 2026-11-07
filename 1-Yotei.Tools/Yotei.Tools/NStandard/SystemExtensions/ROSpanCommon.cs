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
    [SuppressMessage("", "IDE0075")]
    static Predicate<T> InnerEquality<T>(T value) => x =>
        value is IEquatable<T> equatable ? equatable.Equals(x) :
        value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
        false;

    // ----------------------------------------------------

    /// <summary>
    /// Represents a value comparer that either uses the type's capabilities, or the comparer
    /// provided as its argument.
    /// </summary>
    readonly struct ValueComparer<T> : IComparer<T>
    {
        readonly IComparer<T>? _Comparer;

        /// <summary>
        /// Initializes a new instance that only uses the type's capabilities.
        /// </summary>
        public ValueComparer() => _Comparer = null;

        /// <summary>
        /// Initializes a new instance that uses the given comparer.
        /// </summary>
        public ValueComparer(IComparer<T> comparer) => _Comparer = comparer.ThrowWhenNull();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            // A comparer was given...
            if (_Comparer is not null) return _Comparer.Compare(x, y);
            
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
    }

    // ----------------------------------------------------

    /// <summary>
    /// Represents an equality comparer that either uses the type's capabilities, or the comparer
    /// provided as its argument.
    /// </summary>
    readonly struct EqualityComparer<T> : IEqualityComparer<T>
    {
        readonly IEqualityComparer<T>? _Comparer;
        readonly bool _ThrowTypeException;

        /// <summary>
        /// Initiliazes a new instance that uses the type's capabilities.
        /// </summary>
        public EqualityComparer()
        {
            _Comparer = null;
            _ThrowTypeException = false;
        }

        /// <summary>
        /// Initializes a new instance that uses the given comparer. If no comparison can be
        /// performed, the comparer either returns <c>false</c>, or throws a type exception if
        /// such is requested.
        /// </summary>
        public EqualityComparer(IEqualityComparer<T> comparer, bool throwTypeException = false)
        {
            _Comparer = comparer.ThrowWhenNull();
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Equals(T? x, T? y)
        {
            // A comparer was given...
            if (_Comparer is not null) return _Comparer.Equals(x, y);

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