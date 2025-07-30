namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null string value, including empty
/// or spaces-only ones.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class StrTokenText : IStrToken
{
    /// <summary>
    /// A common shared empty instance.
    /// </summary>
    public static StrTokenText Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenText() { }

    /// <summary>
    /// Initializes a new instance with the given not-null payload.
    /// </summary>
    /// <param name="payload"></param>
    public StrTokenText(string payload) => Payload = payload;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc cref="IStrToken.Payload"/>
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    string _Payload = string.Empty;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance shall be considered equal to the other given one, using the
    /// given string comparison mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public virtual bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not StrTokenText valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other) => Equals(other, StringComparison.CurrentCulture);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as StrTokenText);

    // Equality operator.
    public static bool operator ==(StrTokenText? x, IStrToken? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(StrTokenText? x, IStrToken? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode() => Payload.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();
        return tokenizer(Payload);
    }
}