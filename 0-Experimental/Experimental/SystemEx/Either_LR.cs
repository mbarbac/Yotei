namespace Experimental;

// ========================================================
/// <summary>
/// Represents an 'Either' monad, and object that can hold either an instance of its left type
/// or an instance of its right one.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public readonly struct Either<L, R> : IComparable<Either<L, R>>, IEquatable<Either<L, R>>
{
    readonly L _Left = default!;
    readonly R _Right = default!;
    readonly bool _IsLeft;

    /// <summary>
    /// Empty <see cref="Either"/> monads are not allowed.
    /// </summary>
    public Either() => throw new InvalidOperationException(
        "Empty 'Either' monads are not allowed.");

    /// <summary>
    /// Initializes a new L-instance.
    /// </summary>
    /// <param name="value"></param>
    public Either(L value)
    {
        if (typeof(L) == typeof(R)) throw new InvalidOperationException(
            "Left type cannot be the same as the right one.")
            .WithData(typeof(L), nameof(L))
            .WithData(typeof(R), nameof(R));

        _Left = value;
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
            .WithData(typeof(L), nameof(L))
            .WithData(typeof(R), nameof(R));

        _Right = value;
        _IsLeft = false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Match(onLeft: x => $"{x}", onRight: x => $"{x}");

    /// <summary>
    /// Determines if this instance is a L-one, or not.
    /// </summary>
    public readonly bool IsLeft => _IsLeft;

    /// <summary>
    /// Determines if this instance is a R-one, or not.
    /// </summary>
    public readonly bool IsRight => !_IsLeft;

    /// <summary>
    /// Invokes the appropriate delegate depending upon if this instance is an L-one or an R-one.
    /// </summary>
    /// <param name="onLeft"></param>
    /// <param name="onRight"></param>
    public readonly void Match(Action<L> onLeft, Action<R> onRight)
    {
        onLeft.ThrowWhenNull();
        onRight.ThrowWhenNull();

        if (_IsLeft) onLeft(_Left);
        else onRight(_Right);
    }

    /// <summary>
    /// Invokes the appropriate delegate depending upon if this instance is an L-one or an R-one.
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
    /// When the two given instances are of the same underlying type (both are L-ones or R-ones),
    /// this method compares their values and returns and integer that indicates if the left one
    /// precedes (-1), follows (+1), or occurs in the same position (0) as the right one.
    /// <br/> When they are not of the same underlying type then, by convention, those of L-type
    /// precede the R-ones.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Either<L, R> x, Either<L, R> y)
    {
        if (x._IsLeft == y._IsLeft)
        {
            return x.Match(
                onLeft: z => CompareValues(z, y._Left),
                onRight: z => CompareValues(z, y._Right));
        }

        return x._IsLeft ? -1 : +1;
    }

    static int CompareValues<V>(V x, V y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        return x is IComparable<V> comparable
            ? comparable.CompareTo(y)
            : Comparer<V>.Default.Compare(x, y);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Either<L, R> other) => Compare(this, other);

    public static bool operator ==(Either<L, R> x, Either<L, R> y) => Compare(x, y) == 0;
    public static bool operator !=(Either<L, R> x, Either<L, R> y) => Compare(x, y) != 0;
    public static bool operator >(Either<L, R> x, Either<L, R> y) => Compare(x, y) > 0;
    public static bool operator <(Either<L, R> x, Either<L, R> y) => Compare(x, y) < 0;
    public static bool operator >=(Either<L, R> x, Either<L, R> y) => Compare(x, y) >= 0;
    public static bool operator <=(Either<L, R> x, Either<L, R> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given objects shall be considered equal or not.
    /// <br/> When they both are of the same underlying type (both are L-ones or R-ones), then
    /// this method compares their values to determine equality. If they are not of the same type,
    /// they are considered different ones regardless their actual values.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(Either<L, R> x, Either<L, R> y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Either<L, R> other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) return false;
        if (obj is not Either<L, R> other) return false;

        return Equals(other);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode() => Match(
        onLeft: z => z is null ? HashCode.Combine(z) : z.GetHashCode(),
        onRight: z => z is null ? HashCode.Combine(z) : z.GetHashCode());
}