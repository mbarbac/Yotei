namespace Experimental;

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
}