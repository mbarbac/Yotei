namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null and not-empty keyword values that
/// shall not be tokenized any further. Keywords are trimmed before used.
/// </summary>
public class StrTokenKeyword : IStrToken
{
    /// <summary>
    /// Initializes a new instance with the given payload value
    /// </summary>
    /// <param name="payload"></param>
    public StrTokenKeyword(string payload) => Payload = payload;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc cref="IStrToken.Payload"/>
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.NotNullNotEmpty(trim: true);
    }
    string _Payload = default!;
    object? IStrToken.Payload => _Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not StrTokenKeyword valid) return false;

        if (Payload != valid.Payload) return false;
        return true;
    }

    /// <summary>
    /// Indicates whether this instance is equal to the given one, using the given comparison.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not StrTokenKeyword valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrToken);

    // Equality operator.
    public static bool operator ==(StrTokenKeyword? x, IStrToken? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(StrTokenKeyword? x, IStrToken? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode() => Payload.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer) => this;
}