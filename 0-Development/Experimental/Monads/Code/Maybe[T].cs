namespace Experimental.Monads;

// ========================================================
/// <summary>
/// Represents a monad that either holds a valid instance of its generic type, or not.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Maybe<T> : IComparable<Maybe<T>>, IEquatable<Maybe<T>>
{
    /// <summary>
    /// A default invalid instance.
    /// </summary>
    public static Maybe<T> None { get; } = new();

    // ----------------------------------------------------

    readonly T _Value = default!;
    readonly bool _IsValid = false;

    /// <summary>
    /// Initializes an invalid instance.
    /// </summary>
    public Maybe() { }

    /// <summary>
    /// Initializes a valid instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public Maybe(T value)
    {
        _Value = value;
        _IsValid = true;
    }

    /// <inheritdoc/>
    public override string ToString() => _IsValid ? $"'{_Value}'" : "<invalid>";

    /// <summary>
    /// Determines if this instance is a valid one, or not.
    /// </summary>
    public readonly bool IsValid => _IsValid;

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a valid one,
    /// or not.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    public readonly void Match(Action<T> valid, Action invalid)
    {
        valid.ThrowWhenNull();
        invalid.ThrowWhenNull();

        if (_IsValid) valid(_Value); else invalid();
    }

    /// <summary>
    /// Executes the appropriate delegate depending on whether this instance is a valid one,
    /// or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
    /// Implicit conversion from a value to a monad.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Maybe<T>(T value) => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position, in sort order, as the second one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Maybe<T> x, Maybe<T> y)
    {
        if (x.IsValid && y.IsValid) return CompareValues(x._Value, y._Value);

        if (!x.IsValid && !y.IsValid) return 0;
        return !x.IsValid ? -1 : +1;

        // Invoked to compare the actual values...
        static int CompareValues(T x, T y)
        {
            if (x is null && y is null) return 0;

            return x is IComparable comparable
                ? comparable.CompareTo(y)
                : Comparer<T>.Default.Compare(x, y);
        }
    }

    /// <inheritdoc/>
    public int CompareTo(Maybe<T> other) => Compare(this, other);

    public static bool operator ==(Maybe<T> x, Maybe<T> y) => Compare(x, y) == 0;
    public static bool operator !=(Maybe<T> x, Maybe<T> y) => Compare(x, y) != 0;
    public static bool operator >(Maybe<T> x, Maybe<T> y) => Compare(x, y) > 0;
    public static bool operator <(Maybe<T> x, Maybe<T> y) => Compare(x, y) < 0;
    public static bool operator >=(Maybe<T> x, Maybe<T> y) => Compare(x, y) >= 0;
    public static bool operator <=(Maybe<T> x, Maybe<T> y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(Maybe<T> other) => Compare(this, other) == 0;

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) return false;
        if (obj is not Maybe<T> other) return false;

        return Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Match(
        valid: x => HashCode.Combine(x),
        invalid: () => Maybe.None.GetHashCode());

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the actual value carried by this instance, provided it is a valid one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(out T value)
    {
        value = IsValid ? _Value : default!;
        return IsValid;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either the value carried by this instance, if it is a valid one, or otherwise
    /// the given alternate value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public T Or(T value) => IsValid ? _Value : value;

    /// <summary>
    /// Returns either the value carried by this instance, if it is a valid one, or otherwise
    /// the value obtained from executing the given delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Or(Func<T> func)
    {
        func.ThrowWhenNull();
        return IsValid ? _Value : func();
    }

    /// <summary>
    /// Returns either this instance, if it is a valid one, or otherwise the given alternate one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Maybe<T> Or(Maybe<T> other) => IsValid ? this : other;

    /// <summary>
    /// Returns either this instance, if it is a valid one, or otherwise the one obtained from
    /// executing the given delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public Maybe<T> Or(Func<Maybe<T>> func)
    {
        func.ThrowWhenNull();
        return IsValid ? this : func();
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
        return IsValid ? new Maybe<R>(proyector(_Value)) : Maybe.None;
    }

    /// <summary>
    /// If this instance is a valid one, returns a new monad that carries the value obtained by
    /// converting the original instance using the given proyector delegate. Otherwise, returns
    /// an invalid instance.
    /// <br/> This method implements the 'bind' monad capability.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="proyector"></param>
    /// <returns></returns>
    public Maybe<R> SelectMany<R>(Func<T, Maybe<R>> proyector)
    {
        proyector.ThrowWhenNull();
        return IsValid ? proyector(_Value) : Maybe.None;
    }

    /// <summary>
    /// If this instance is a valid one, returns a new monad that carries the value obtained from
    /// using the original one filtered by the given selector delegate, and then converted using
    /// the given proyector one. Otherwise, returns an invalid instance.
    /// <br/> This method implements the 'bind' monad capability.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="selector"></param>
    /// <param name="proyector"></param>
    /// <returns></returns>
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
}