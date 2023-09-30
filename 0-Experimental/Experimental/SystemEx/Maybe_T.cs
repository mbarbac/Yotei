namespace Experimental;

// ========================================================
/// <summary>
/// Represents a 'Maybe' monad, and object that can hold either a valid value or not.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Maybe<T> : IComparable<Maybe<T>>, IEquatable<Maybe<T>>
{
    readonly T _Value;
    readonly bool _IsValid;

    /// <summary>
    /// A shared invalid instance.
    /// </summary>
    public static Maybe<T> None { get; } = new();

    /// <summary>
    /// Initializes a new empty and invalid instance.
    /// </summary>
    public Maybe()
    {
        _Value = default!;
        _IsValid = false;
    }

    /// <summary>
    /// Initializes a new instance that carries the given value.
    /// </summary>
    /// <param name="value"></param>
    public Maybe(T value)
    {
        _Value = value;
        _IsValid = true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _IsValid ? $"{_Value}" : "<invalid>";

    /// <summary>
    /// Determines if this instance is a valid one, or not.
    /// </summary>
    public readonly bool IsValid => _IsValid;

    /// <summary>
    /// Invokes the appropriate delegate depending upon if this instance is a valid or an invalid
    /// one.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    public readonly void Match(Action<T> valid, Action invalid)
    {
        valid.ThrowWhenNull();
        invalid.ThrowWhenNull();

        if (_IsValid) valid(_Value);
        else invalid();
    }

    /// <summary>
    /// Invokes the appropriate delegate depending upon if this instance is a valid or an invalid
    /// one.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    /// <returns></returns>
    public readonly V Match<V>(Func<T, V> valid, Func<V> invalid)
    {
        valid.ThrowWhenNull();
        invalid.ThrowWhenNull();

        return _IsValid ? valid(_Value) : invalid();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Implicit conversion from an invalid monad.
    /// </summary>
    /// <param name="_"></param>
    public static implicit operator Maybe<T>(Maybe.Invalid _) => new();

    /// <summary>
    /// Implicit conversion from a value to a valid monad.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Maybe<T>(T value) => new(value);

    /// <summary>
    /// Tries to get the value of this instance, provided it is a valid one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(out T value)
    {
        value = _IsValid ? _Value : default!;
        return _IsValid;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the value carried by this instance if it is a valid one or, otherwise, returns
    /// the alternate value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public T Or(T value) => _IsValid ? _Value : value;

    /// <summary>
    /// Returns the value carried by this instance if it is a valid one or, otherwise, returns
    /// the alternate value obtained from the given delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Or(Func<T> func)
    {
        func.ThrowWhenNull();
        return _IsValid ? _Value : func();
    }

    /// <summary>
    /// Returns this instance if it is a valid one, or otherwise the alternate one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Maybe<T> Or(Maybe<T> other) => _IsValid ? this : other;

    /// <summary>
    /// Returns this instance if it is a valid one, or otherwise the alternate one obtained from
    /// the given delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public Maybe<T> Or(Func<Maybe<T>> func)
    {
        func.ThrowWhenNull();
        return _IsValid ? _Value : func();
    }

    // ----------------------------------------------------

    /// <summary>
    /// If this instance is a valid one, returns a new monad that carries the value obtained by
    /// converting the original one using the given proyector delegate. Otherwise, returns an
    /// invalid instance.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="proyector"></param>
    /// <returns></returns>
    public Maybe<R> Select<R>(Func<T, R> proyector)
    {
        proyector.ThrowWhenNull();
        return _IsValid ? new Maybe<R>(proyector(_Value)) : Maybe.None;
    }

    /// <summary>
    /// If this instance is a valid one, returns a new one obtained using the given proyector
    /// delegate with the original value. Otherwise, returns an invalid instance.
    /// <br/> This method implements the 'bind' monad capability.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="proyector"></param>
    /// <returns></returns>
    public Maybe<R> SelectMany<R>(Func<T, Maybe<R>> proyector)
    {
        proyector.ThrowWhenNull();
        return _IsValid ? proyector(_Value) : Maybe.None;
    }

    /// <summary>
    /// If this instance is a valid one, returns a new one that carries the value obtained from
    /// using the original one filtered by the given selector delegate, and then converted using
    /// the given proyector one. Otherwise, returns an invalid instance.
    /// <br/> This method implements the 'bind' monad capability.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="selector"></param>
    /// <param name="proyector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public Maybe<R> SelectMany<U, R>(Func<T, Maybe<U>> selector, Func<T, U, R> proyector)
    {
        selector.ThrowWhenNull();
        proyector.ThrowWhenNull();

        var temp = selector(_Value); if (!temp.IsValid) return Maybe.None;
        try
        {
            var other = proyector(_Value, temp._Value);
            var item = new Maybe<R>(other);
            return item;
        }
        catch { }
        return Maybe.None;
    }

    // ----------------------------------------------------

    /// <summary>
    /// When the two given instance are valid ones, this method compares their values and returns
    /// and integer that indicates if the left one precedes (-1), follows (+1), or occurs in the
    /// same position (0) as the right one.
    /// <br/> When any is an invalid instance then, by convention, it always precedes a valid one.
    /// If both are invalid, then equality is reported.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Maybe<T> x, Maybe<T> y)
    {
        if (x._IsValid && y._IsValid)
        {
            return CompareValues(x._Value, y._Value);
        }
        if (!x._IsValid && !y._IsValid)
        {
            return 0;
        }
        return !x._IsValid ? -1 : 1;
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
    public int CompareTo(Maybe<T> other) => Compare(this, other);

    public static bool operator ==(Maybe<T> x, Maybe<T> y) => Compare(x, y) == 0;
    public static bool operator !=(Maybe<T> x, Maybe<T> y) => Compare(x, y) != 0;
    public static bool operator >(Maybe<T> x, Maybe<T> y) => Compare(x, y) > 0;
    public static bool operator <(Maybe<T> x, Maybe<T> y) => Compare(x, y) < 0;
    public static bool operator >=(Maybe<T> x, Maybe<T> y) => Compare(x, y) >= 0;
    public static bool operator <=(Maybe<T> x, Maybe<T> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given objects shall be considered equal or not.
    /// <br/> When they are both valid instances, then this method compares their values in order
    /// to determine equality. If both are invalid, they are considered equal. If any is a valid
    /// one and the other is invalid, then they are always considered different.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(Maybe<T> x, Maybe<T> y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Maybe<T> other) => Compare(this, other) == 0;

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
    public override readonly int GetHashCode() => _IsValid
        ? (_Value is null ? HashCode.Combine(_Value) : _Value.GetHashCode())
        : 0;
}