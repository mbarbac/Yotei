#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Builds <see cref="IEqualityComparer{T}"/> instances.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
static class EqualityComparerEx
{
    /// <summary>
    /// Returns a new instance that uses the type's capabilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="throwTypeException">
    /// Throws a <see cref="TypeException"/> instead of an
    /// <see cref="ArgumentException"/> one when needed.</param>
    /// <returns></returns>
    public static IEqualityComparer<T> New<T>(
        bool throwTypeException = false) => new InnerComparer<T>(throwTypeException);

    /// <summary>
    /// Returns a new instance that uses the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comparer"></param>
    /// <param name="throwTypeException">
    /// Throws a <see cref="TypeException"/> instead of an
    /// <see cref="ArgumentException"/> one when needed.</param>
    /// <returns></returns>
    public static IEqualityComparer<T> New<T>(
        IEqualityComparer<T> comparer,
        bool throwTypeException = false) => new InnerComparer<T>(comparer, throwTypeException);

    /// <summary>
    /// Returns a new instance that uses the given delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="throwTypeException">
    /// Throws a <see cref="TypeException"/> instead of an
    /// <see cref="ArgumentException"/> one when needed.</param>
    /// <returns></returns>
    public static IEqualityComparer<T> New<T>(
        Func<T?, T?, bool> func,
        bool throwTypeException = false) => new InnerComparer<T>(func, throwTypeException);

    // ----------------------------------------------------

    readonly struct InnerComparer<T> : IEqualityComparer<T>
    {
        readonly IEqualityComparer<T>? _Comparer;
        readonly Func<T?, T?, bool>? _Func;
        readonly bool _ThrowTypeException;

        /// <summary>
        /// Initializes a new instance that uses the type's capabilities.
        /// </summary>
        public InnerComparer(bool throwTypeException)
        {
            _Comparer = null;
            _Func = null;
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// Initializes a new instance that uses the given comparer.
        /// </summary>
        public InnerComparer(IEqualityComparer<T> comparer, bool throwTypeException)
        {
            _Comparer = comparer.ThrowWhenNull();
            _Func = null;
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// Initializes a new instance that uses the given delegate.
        /// </summary>
        public InnerComparer(Func<T?, T?, bool> func, bool throwTypeException)
        {
            _Comparer = null;
            _Func = func.ThrowWhenNull();
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Equals(T? x, T? y)
        {
            // External elements...
            if (_Comparer is not null) return _Comparer.Equals(x, y);
            if (_Func is not null) return _Func(x, y);


            // Comparison against null...
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

            if (_ThrowTypeException) // Only if explicitly requested...
            {
                throw new TypeException(
                "Type does not implement IComparable<T> nor IEquatable<T>.")
                .WithData(type)
                .WithData(x)
                .WithData(y);
            }
            else // Mimicing standard behavior...
            {
                return false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;
    }
}