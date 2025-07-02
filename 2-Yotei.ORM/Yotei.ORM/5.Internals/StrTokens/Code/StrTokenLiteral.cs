namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null and not-empty string value that
/// will not be tokenized any further. Note that spaces-only instances are considered valid.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
public class StrTokenLiteral : IStrToken
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenLiteral() { }

    /// <summary>
    /// Initializes a new instance with the given not-null and not-empty payload. Note that
    /// spaces-only instances are considered valid.
    /// </summary>
    /// <param name="payload"></param>
    public StrTokenLiteral(string payload) => Payload = payload;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc cref="IStrToken.Payload"/>
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.NotNullNotEmpty(trim: false);
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
        if (other is null || other is not StrTokenLiteral valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other) => Equals(other, StringComparison.CurrentCulture);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as StrTokenLiteral);

    // Equality operator.
    public static bool operator ==(StrTokenLiteral? x, IStrToken? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(StrTokenLiteral? x, IStrToken? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode() => Payload.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer) => this;
}