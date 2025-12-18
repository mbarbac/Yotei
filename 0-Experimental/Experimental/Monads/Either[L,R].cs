namespace Experimental;

// ========================================================
/// <summary>
/// Represents a monad that holds either an instance of its left type, or an instance of its
/// right one.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public readonly struct Either<L, R> : IComparable<Either<L, R>>, IEquatable<Either<L, R>>
{
    readonly L _Left = default!;
    readonly R _Right = default!;
    readonly bool _IsLeft;

    /// <summary>
    /// Empty monad not allowed.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Either() => throw new InvalidOperationException("Empty Either monads not allowed.");

    /// <summary>
    /// Initializes a new L-instance.
    /// </summary>
    /// <param name="value"></param>
    public Either(L value)
    {
        if (typeof(L) == typeof(R)) throw new InvalidOperationException(
            "Left type cannot be the same as the right one.");

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
            "Left type cannot be the same as the right one.");

        _Right = value;
        _IsLeft = false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// Invokes the appropriate delegate depending on whether this is an L-instance or an R-one. 
    /// </summary>
    /// <param name="onLeft"></param>
    /// <param name="onRight"></param>
    public readonly void Match(Action<L> onLeft, Action<R> onRight)
    {
        ArgumentNullException.ThrowIfNull(onLeft);
        ArgumentNullException.ThrowIfNull(onRight);

        if (_IsLeft) onLeft(_Left); else onRight(_Right);
    }

    /// <summary>
    /// Invokes the appropriate delegate depending on whether this is an L-instance or an R-one. 
    /// </summary>
    /// <param name="onLeft"></param>
    /// <param name="onRight"></param>
    public readonly T Match<T>(Func<L, T> onLeft, Func<R, T> onRight)
    {
        ArgumentNullException.ThrowIfNull(onLeft);
        ArgumentNullException.ThrowIfNull(onRight);

        return _IsLeft ? onLeft(_Left) : onRight(_Right);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns -1 if the first one precedes the second one
    /// in sort order, 0 if both occurs in the same sort order, or +1 if the first one follows
    /// the second one in sort order.
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

        return x is IComparable comparable
            ? comparable.CompareTo(y)
            : Comparer<V>.Default.Compare(x, y);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(Either<L, R> other) => Compare(this, other);

    public static bool operator ==(Either<L, R> x, Either<L, R> y) => Compare(x, y) == 0;
    public static bool operator !=(Either<L, R> x, Either<L, R> y) => Compare(x, y) != 0;
    public static bool operator >(Either<L, R> x, Either<L, R> y) => Compare(x, y) > 0;
    public static bool operator <(Either<L, R> x, Either<L, R> y) => Compare(x, y) < 0;
    public static bool operator >=(Either<L, R> x, Either<L, R> y) => Compare(x, y) >= 0;
    public static bool operator <=(Either<L, R> x, Either<L, R> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public bool Equals(Either<L, R> other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) return false;
        if (obj is not Either<L, R> other) return false;

        return Equals(other);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode() => Match(
        onLeft: z => z is null ? HashCode.Combine(z) : z.GetHashCode(),
        onRight: z => z is null ? HashCode.Combine(z) : z.GetHashCode());
}