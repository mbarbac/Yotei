namespace Experimental.Monads;

// ========================================================
/// <summary>
/// Represents a monad that acts as a union, holding either an instance of its left type, or an
/// instance of its right one. Both types must not be the same, or an exception is thrown.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public readonly struct Either<L, R> : IComparable<Either<L, R>>, IEquatable<Either<L, R>>
{
    readonly L _Left;
    readonly R _Right;
    readonly bool _IsLeft;

    /// <summary>
    /// Empty constructor is not allowed.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Either() => throw new InvalidOperationException("Empty 'Either' monads are not allowed.");

    /// <summary>
    /// Initializes a new L-instance.
    /// </summary>
    /// <param name="value"></param>
    public Either(L value)
    {
        if (typeof(L) == typeof(R)) throw new InvalidOperationException(
            "Left type cannot be the same as the right one.")
            .WithData(typeof(L), "Left Type")
            .WithData(typeof(R), "Right Type");

        _Left = value;
        _Right = default!;
        _IsLeft = true;
    }

    /// <summary>
    /// Initializes a new R-instance.
    /// </summary>
    /// <param name="value"></param>
    public Either(R value)
    {
        if (typeof(L) == typeof(R)) throw new InvalidOperationException(
            "Left type cannot be the same as the right one.")
            .WithData(typeof(L), "Left Type")
            .WithData(typeof(R), "Right Type");

        _Left = default!;
        _Right = value;
        _IsLeft = false;
    }

    /// <inheritdoc/>
    public override string ToString() => Match(onLeft: x => $"'{x}'", onRight: x => $"'{x}'");

    /// <summary>
    /// Determines if this instance is a L-one, or not.
    /// </summary>
    public readonly bool IsLeft => _IsLeft;

    /// <summary>
    /// Determines if this instance is a R-one, or not.
    /// </summary>
    public readonly bool IsRight => !_IsLeft;

    // ----------------------------------------------------

    /// <summary>
    /// Invokes the appropriate delegate with the appropriate vale based upon if this instance
    /// is an L or an R one.
    /// </summary>
    /// <param name="onLeft"></param>
    /// <param name="onRight"></param>
    public readonly void Match(Action<L> onLeft, Action<R> onRight)
    {
        onLeft.ThrowWhenNull();
        onRight.ThrowWhenNull();

        if (_IsLeft) onLeft(_Left); else onRight(_Right);
    }

    /// <summary>
    /// Invokes the appropriate delegate with the appropriate vale based upon if this instance
    /// is an L or an R one, and returns the result of that invocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="onLeft"></param>
    /// <param name="onRight"></param>
    /// <returns></returns>
    public readonly T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
    {
        onLeft.ThrowWhenNull();
        onRight.ThrowWhenNull();

        return _IsLeft ? onLeft(_Left) : onRight(_Right);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position, in sort order, as the second one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Either<L, R> x, Either<L, R> y)
    {
        if (x.IsLeft == y.IsLeft) return x.Match(
            onLeft: z => CompareValues(z, y._Left),
            onRight: z => CompareValues(z, y._Right));

        return x.IsLeft ? -1 : +1;

        // Invoked to compare the actual values...
        static int CompareValues<T>(T x, T y)
        {
            if (x is null && y is null) return 0;

            return x is IComparable comparable
                ? comparable.CompareTo(y)
                : Comparer<T>.Default.Compare(x, y);
        }
    }

    /// <inheritdoc/>
    public int CompareTo(Either<L, R> other) => Compare(this, other);

    public static bool operator ==(Either<L, R> x, Either<L, R> y) => Compare(x, y) == 0;
    public static bool operator !=(Either<L, R> x, Either<L, R> y) => Compare(x, y) != 0;
    public static bool operator >(Either<L, R> x, Either<L, R> y) => Compare(x, y) > 0;
    public static bool operator <(Either<L, R> x, Either<L, R> y) => Compare(x, y) < 0;
    public static bool operator >=(Either<L, R> x, Either<L, R> y) => Compare(x, y) >= 0;
    public static bool operator <=(Either<L, R> x, Either<L, R> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(Either<L, R> other) => Compare(this, other) == 0;

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) return false;
        if (obj is not Either<L, R> other) return false;

        return Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Match(
        onLeft: z => z is null ? HashCode.Combine(z) : z.GetHashCode(),
        onRight: z => z is null ? HashCode.Combine(z) : z.GetHashCode());
}