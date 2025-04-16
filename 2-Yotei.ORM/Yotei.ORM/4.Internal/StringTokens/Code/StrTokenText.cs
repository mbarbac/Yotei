namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null string value.
/// </summary>
public class StrTokenText : IStrToken
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static StrTokenText Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenText() : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance with the given payload value
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
    string _Payload = default!;
    object? IStrToken.Payload => _Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IStrToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not StrTokenText valid) return false;

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
        if (other is null || other is not StrTokenText valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        return tokenizer.ThrowWhenNull()(_Payload);
    }
}