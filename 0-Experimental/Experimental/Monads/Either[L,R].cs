using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;

namespace Experimental;

// ========================================================
/// <summary>
/// Represents an object that either holds a value of its left type, or a value of its right one.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public readonly struct Either<L, R> : IComparable<Either<L, R>>, IEquatable<Either<L, R>>
{
    readonly L _Left = default!;
    readonly R _Right = default!;
    readonly bool _IsLeft;

    /// <summary>
    /// Invalid empty constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Either() => throw new InvalidOperationException("Empty 'Either<L,R>' monads not allowed.");

    /// <summary>
    /// Initializes a new L-instance.
    /// </summary>
    /// <param name="value"></param>
    public Either(L value)
    {
        if (typeof(L) == typeof(R)) throw new InvalidOperationException(
            "Left type cannot be the same as the right one.")
            .WithData(typeof(L), "Left")
            .WithData(typeof(R), "Right");

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
            .WithData(typeof(L), "Left")
            .WithData(typeof(R), "Right");

        _Right = value;
        _IsLeft = false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Match(
        onleft: static x => $"{x}",
        onright: static x => $"{x}");

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is an L-one, or not.
    /// </summary>
    public readonly bool IsLeft => _IsLeft;

    /// <summary>
    /// Determines if this instance is an R-one, or not.
    /// </summary>
    public readonly bool IsRight => !_IsLeft;

    // ----------------------------------------------------

    /// <summary>
    /// Compares the left instance with the right one and returns an integer whose value indicates
    /// whether the left instance precedes, follows, or occurs in the same position as the right
    /// one in sort order:
    /// <br/>- Less than cero: the left instance precedes the right one.
    /// <br/>- Zero: both instances occur at the same position.
    /// <br/>- Greater than cero: the left instance follows the right one.
    /// <para>
    /// If the instances to compare are not of the same kind (both are not L-ones or R-ones), then
    /// L-ones take priority.
    /// </para>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static int Compare(Either<L, R> left, Either<L, R> right)
    {
        if (left._IsLeft == right.IsLeft) return left.Match(
            onleft: x => CompareValues(x, right._Left),
            onright: x => CompareValues(x, right._Right));

        return left._IsLeft ? -1 : +1;
    }

    static int CompareValues<T>(T left, T right)
    {
        if (!typeof(T).IsValueType)
        {
            if (left is null && right is null) return 0;
            if (left is null) return +1;
            if (right is null) return -1;
        }

        return left is IComparable<T> item
            ? item.CompareTo(right)
            : Comparer<T>.Default.Compare(left, right);
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
    /// Indicates whether the left value is equal to the right one, or not.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool Equals(Either<L, R> left, Either<L, R> right)
    {
        return (left._IsLeft, right._IsLeft) switch
        {
            (true, true) => EqualValues(left._Left, right._Left),
            (false, false) => EqualValues(left._Right, right._Right),
            _ => false
        };
    }

    static bool EqualValues<T>(T left, T right)
    {
        if (!typeof(T).IsValueType)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
        }

        return left is IEquatable<T> item
            ? item.Equals(right)
            : CompareValues(left, right) == 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Either<L, R> other) => Equals(this, other);

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
    public override int GetHashCode() => Match(
        onleft: x => x is null ? HashCode.Combine(x) : x.GetHashCode(),
        onright: x => x is null ? HashCode.Combine(x) : x.GetHashCode());

    // ----------------------------------------------------

    /// <summary>
    /// Implicit conversion operator to a L-instance.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Either<L, R>(L value) => new(value);

    /// <summary>
    /// Implicit conversion operator to a R-instance.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Either<L, R>(R value) => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a L or an R one.
    /// </summary>
    /// <param name="onleft"></param>
    /// <param name="onright"></param>
    public readonly void Match(Action<L> onleft, Action<R> onright)
    {
        ArgumentNullException.ThrowIfNull(onleft);
        ArgumentNullException.ThrowIfNull(onright);

        if (_IsLeft) onleft(_Left); else onright(_Right);
    }

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a L or an R one.
    /// </summary>
    /// <param name="onleft"></param>
    /// <param name="onright"></param>
    public readonly T Match<T>(Func<L, T> onleft, Func<R, T> onright)
    {
        ArgumentNullException.ThrowIfNull(onleft);
        ArgumentNullException.ThrowIfNull(onright);

        return _IsLeft ? onleft(_Left) : onright(_Right);
    }
}