namespace Experimental;

// ========================================================
/// <summary>
/// Represents a monad that either holds a value of its associated type, or not.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Maybe<T> : IComparable<Maybe<T>>, IEquatable<Maybe<T>>
{
    readonly T _Value = default!;
    readonly bool _Valid = false;

    /// <summary>
    /// A default shared invalid instance.
    /// </summary>
    public static Maybe<T> None { get; } = new();

    /// <summary>
    /// Initializes an invalid instance, equivalent to a '<see cref="None"/>' one.
    /// </summary>
    public Maybe() { }

    /// <summary>
    /// Initializes a valid instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public Maybe(T value)
    {
        _Value = value;
        _Valid = true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _Valid ? $"{_Valid}" : "<invalid>";

    // ----------------------------------------------------

    /// <summary>
    /// Compares the left instance with the right one and returns an integer whose value indicates
    /// whether the left instance precedes, follows, or occurs in the same position as the right
    /// one in sort order:
    /// <br/>- Less than cero: the left instance precedes the right one.
    /// <br/>- Zero: both instances occur at the same position.
    /// <br/>- Greater than cero: the left instance follows the right one.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static int Compare(Maybe<T> left, Maybe<T> right)
    {
        if (!left._Valid && !right._Valid) return 0;
        if (!left._Valid) return +1;
        if (!right._Valid) return -1;

        return CompareValues(left._Value, right._Value);
    }

    static int CompareValues<V>(V left, V right)
    {
        if (!typeof(V).IsValueType)
        {
            if (left is null && right is null) return 0;
            if (left is null) return +1;
            if (right is null) return -1;
        }

        return left is IComparable<V> item
            ? item.CompareTo(right)
            : Comparer<V>.Default.Compare(left, right);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(Maybe<T> other) => Compare(this, other);

    public static bool operator ==(Maybe<T> x, Maybe<T> y) => Compare(x, y) == 0;
    public static bool operator !=(Maybe<T> x, Maybe<T> y) => Compare(x, y) != 0;
    public static bool operator >(Maybe<T> x, Maybe<T> y) => Compare(x, y) > 0;
    public static bool operator <(Maybe<T> x, Maybe<T> y) => Compare(x, y) < 0;
    public static bool operator >=(Maybe<T> x, Maybe<T> y) => Compare(x, y) >= 0;
    public static bool operator <=(Maybe<T> x, Maybe<T> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Indicates whether the left value is equal to the right one, or not.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool Equals(Maybe<T> left, Maybe<T> right)
    {
        if (!left._Valid && !right._Valid) return true;
        if (!left._Valid || !right._Valid) return false;

        var vleft = left._Value;
        var vright = right._Value;

        if (typeof(T).IsValueType)
        {
            if (vleft is null && vright is null) return true;
            if (vleft is null || vright is null) return false;
        }

        return vleft is IEquatable<T> item
            ? item.Equals(vright)
            : left.CompareTo(right) == 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Maybe<T> other) => Equals(this, other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) return false;
        if (obj is not Maybe<T> other) return false;

        return Equals(other);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Match(
        valid: x => x is null ? HashCode.Combine(x) : x.GetHashCode(),
        invalid: static () => Maybe.None.GetHashCode());

    // ----------------------------------------------------

    /// <summary>
    /// Implicit conversion to a monad instances.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Maybe<T>(T value) => new(value);

    /// <summary>
    /// Explicit conversion to a type's instance. This operator throws an exception if the monad
    /// is an invalid one.
    /// </summary>
    /// <param name="item"></param>
    public static explicit operator T(Maybe<T> item) => item._Valid
        ? item._Value
        : throw new InvalidCastException("Cannot convert from an invalid monad").WithData(item);

    /// <summary>
    /// Implicit conversion from an invalid monad.
    /// </summary>
    /// <param name="_"></param>
    public static implicit operator Maybe<T>(Maybe.Invalid _) => new();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is a valid one or not.
    /// </summary>
    public readonly bool Valid => _Valid;

    /// <summary>
    /// Returns either the value carried by this instance, if it is a valid one, or the given
    /// alternate value otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public T Or(T value) => _Valid ? _Value : value;

    /// <summary>
    /// Obtains the value held by this instance, provided that it is a valid one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(out T value)
    {
        value = _Valid ? _Value : default!;
        return _Valid;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a valid one or not.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    public readonly void Match(Action<T> valid, Action invalid)
    {
        ArgumentNullException.ThrowIfNull(valid);
        ArgumentNullException.ThrowIfNull(invalid);

        if (_Valid) valid(_Value); else invalid();
    }

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a valid one or not.
    /// Returns the result of that execution.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    public readonly V Match<V>(Func<T, V> valid, Func<V> invalid)
    {
        ArgumentNullException.ThrowIfNull(valid);
        ArgumentNullException.ThrowIfNull(invalid);

        return _Valid ? valid(_Value) : invalid();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance projecting the valid value carried by this instance into the
    /// result of the given delegate, or an invalid instance otherwise.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="projector"></param>
    /// <returns></returns>
    public Maybe<R> Select<R>(Func<T, R> projector)
    {
        ArgumentNullException.ThrowIfNull(projector);
        return _Valid ? new(projector(_Value)) : new();
    }

    /// <summary>
    /// Returns either a new instance projecting the valid value carried by this instance into the
    /// result of the given delegate, or an invalid instance otherwise.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="converter"></param>
    /// <returns></returns>
    public Maybe<R> Select<R>(Func<T, Maybe<R>> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        return _Valid ? converter(_Value) : new();
    }

    /// <summary>
    /// Returns either a new instance that carries the value obtained from using the original one,
    /// filtered by the given selector delegate, and then converted by the given proyection, or an
    /// invalid instance otherwise.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="selector"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public Maybe<R> SelectMany<U, R>(Func<T, Maybe<U>> selector, Func<T, U, R> converter)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(converter);

        if (!_Valid) return new();

        var temp = selector(_Value); if (!temp._Valid) return new();
        try
        {
            var other = converter(_Value, temp._Value);
            var item = new Maybe<R>(other);
            return item;
        }
        catch { }
        return new();
    }
}