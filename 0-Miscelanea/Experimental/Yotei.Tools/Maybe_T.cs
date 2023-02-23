namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a <see cref="Maybe{T}"/> monad.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Maybe<T> : IComparable<Maybe<T>>, IEquatable<Maybe<T>>
{
    /// <summary>
    /// A shared invalid monad.
    /// </summary>
    public static Maybe<T> None { get; } = new();

    // ----------------------------------------------------

    readonly T _Value = default!;
    readonly bool _IsValid = false;

    /// <summary>
    /// Initializes a new invalid monad.
    /// </summary>
    public Maybe() { }

    /// <summary>
    /// Initializes a new monad that carries the given value.
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
    /// <returns><inheritdoc/></returns>
    public override string ToString() => _IsValid ? $"'{_Value}'" : "<Invalid>";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(Maybe<T> other)
    {
        if (!IsValid && !other.IsValid) return 0;
        if (!IsValid) return -1;
        if (!other.IsValid) return +1;

        if (_Value is IEquatable<T> equatable)
        {
            var r = equatable.Equals(other._Value);
            if (r) return 0;
        }

        if (_Value is IComparable<T> comparable)
        {
            var r = comparable.CompareTo(other._Value);
            return r;
        }

        else
        {
            var c = Comparer<T>.Default;
            var r = c.Compare(_Value, other._Value);
            return r;
        }
    }

    public static bool operator <(Maybe<T> x, Maybe<T> y) => x.CompareTo(y) < 0;
    public static bool operator >(Maybe<T> x, Maybe<T> y) => x.CompareTo(y) > 0;
    public static bool operator <=(Maybe<T> x, Maybe<T> y) => x.CompareTo(y) <= 0;
    public static bool operator >=(Maybe<T> x, Maybe<T> y) => x.CompareTo(y) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool Equals(Maybe<T> other) => CompareTo(other) == 0;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Maybe<T> other) return Equals(other);
        return false;
    }

    public static bool operator ==(Maybe<T> x, Maybe<T> y) => x.Equals(y);
    public static bool operator !=(Maybe<T> x, Maybe<T> y) => !x.Equals(y);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode()
    {
        return _IsValid
            ? (_Value is null ? HashCode.Combine(_Value) : _Value.GetHashCode())
            : 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this monad is a valid one, or not.
    /// </summary>
    public bool IsValid => _IsValid;

    /// <summary>
    /// Implicitly converts from an invalid monad.
    /// </summary>
    /// <param name="_"></param>
    public static implicit operator Maybe<T>(Maybe.Invalid _) => new();

    /// <summary>
    /// Implicitly converts from a given value to a monad.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Maybe<T>(T value) => new(value);

    /// <summary>
    /// Gets the value carried by this monad, if it is a valid one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(out T value)
    {
        value = _IsValid ? _Value : default!;
        return _IsValid;
    }

    /// <summary>
    /// Executes the valid or the invalid delegate depending upon if this monad is a valid one
    /// or not.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    public void Match(Action<T> valid, Action invalid)
    {
        valid.ThrowIfNull();
        invalid.ThrowIfNull();

        if (_IsValid) valid(_Value);
        else invalid();
    }

    /// <summary>
    /// Executes the valid or the invalid delegate depending upon if this monad is a valid one
    /// or not.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="valid"></param>
    /// <param name="invalid"></param>
    /// <returns></returns>
    public R Match<R>(Func<T, R> valid, Func<R> invalid)
    {
        valid.ThrowIfNull();
        invalid.ThrowIfNull();

        return _IsValid ? valid(_Value) : invalid();
    }

    /// <summary>
    /// Returns the value carried by this monad, if it is a valid one, or the alternate value
    /// otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public T Or(T value) => _IsValid ? _Value : value;

    /// <summary>
    /// Returns the value carried by this monad, if it is a valid one, or otherwise the value
    /// obtained from the given delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Or(Func<T> func)
    {
        func = func.ThrowIfNull();
        return _IsValid ? _Value : func();
    }

    /// <summary>
    /// Returns this monad, if it is a valid one, or the alternate one otherwise.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Maybe<T> Or(Maybe<T> other) => _IsValid ? this : other;

    /// <summary>
    /// Returns this monad, if it is a valid one, or otherwise the one obtained from the given
    /// delegate.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public Maybe<T> Or(Func<Maybe<T>> func)
    {
        func = func.ThrowIfNull();
        return _IsValid ? this : func();
    }

    /// <summary>
    /// If this monad is a valid one, returns a new one that carries the value converted using
    /// the given proyector delegate. Otherwise, returns an invalid one.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="proyector"></param>
    /// <returns></returns>
    public Maybe<R> Select<R>(Func<T, R> proyector)
    {
        proyector = proyector.ThrowIfNull();
        return _IsValid ? new Maybe<R>(proyector(_Value)) : Maybe.None;
    }

    /// <summary>
    /// Implements the 'Bind' function. If this monad is a valid one, returns the monad obtained
    /// from the given delegate. Otherwise, returns an invalid one.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="proyector"></param>
    /// <returns></returns>
    public Maybe<R> SelectMany<R>(Func<T, Maybe<R>> proyector)
    {
        proyector = proyector.ThrowIfNull();
        return _IsValid ? proyector(_Value) : Maybe.None;
    }

    /// <summary>
    /// Implements the 'Bind' function. If this monad is a valid one, returns a new one that
    /// carries the value filtered by the given selector, converted using the given proyector.
    /// Otherwise, returns an invalid one.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="selector"></param>
    /// <param name="proyector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public Maybe<R> SelectMany<U, R>(Func<T, Maybe<U>> selector, Func<T, U, R> proyector)
    {
        selector = selector.ThrowIfNull();
        proyector = proyector.ThrowIfNull();

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