#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Builds <see cref="IComparer{T}"/> instances.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
static class ComparerEx
{
    /// <summary>
    /// Returns a new instance that uses the type's capabilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="throwTypeException">
    /// Throws a <see cref="TypeException"/> instead of an
    /// <see cref="ArgumentException"/> one when needed.</param>
    /// <returns></returns>
    public static IComparer<T> New<T>(
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
    public static IComparer<T> New<T>(
        IComparer<T> comparer,
        bool throwTypeException= false) => new InnerComparer<T>(comparer, throwTypeException);

    /// <summary>
    /// Returns a new instance that uses the given delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="throwTypeException">
    /// Throws a <see cref="TypeException"/> instead of an
    /// <see cref="ArgumentException"/> one when needed.</param>
    /// <returns></returns>
    public static IComparer<T> New<T>(
        Func<T?, T?, int> func,
        bool throwTypeException = false) => new InnerComparer<T>(func, throwTypeException);

    // ----------------------------------------------------

    readonly struct InnerComparer<T> : IComparer<T>
    {
        readonly IComparer<T>? _Comparer;
        readonly Func<T?, T?, int>? _Func;
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
        public InnerComparer(IComparer<T> comparer, bool throwTypeException)
        {
            _Comparer = comparer.ThrowWhenNull();
            _Func = null;
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// Initializes a new instance that uses the given delegate.
        /// </summary>
        public InnerComparer(Func<T?, T?, int> func, bool throwTypeException)
        {
            _Comparer = null;
            _Func = func.ThrowWhenNull();
            _ThrowTypeException = throwTypeException;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            // External elements...
            if (_Comparer is not null) return _Comparer.Compare(x, y);
            if (_Func is not null) return _Func(x, y);

            // Comparison against null...
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;

            // Type implements IComparable<T>...
            var type = typeof(T);
            if (typeof(IComparable<T>).IsAssignableFrom(type))
            {
                var data = ((IComparable<T>)x).CompareTo(y);
                if (data < 0) return -1;
                if (data > 0) return +1;
                return 0;
            }

            // Type implements IEquatable<T>...
            if (typeof(IEquatable<T>).IsAssignableFrom(type))
            {
                var data = ((IEquatable<T>)x).Equals(y);
                if (data) return 0;
            }

            if (_ThrowTypeException) // Only if explicitly requested...
            {
                throw new TypeException(
                "Type does not implement IComparable<T>, and instances are not equal.")
                .WithData(type)
                .WithData(x)
                .WithData(y);
            }
            else // Throwing an 'ArgumentException' to mimic standard behavior...
            {
                throw new ArgumentException(
                "Type does not implement IComparable<T>, and instances are not equal.")
                .WithData(type)
                .WithData(x)
                .WithData(y);
            }
        }
    }
}