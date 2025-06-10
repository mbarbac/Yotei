namespace Yotei.ORM.Internals;

// ========================================================
/// <inheritdoc cref="IStrTokenLiteral"/>
public class StrTokenLiteral : IStrTokenLiteral
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenLiteral() { }

    /// <summary>
    /// Initializes a new instance with the given not-null payload.
    /// </summary>
    /// <param name="payload"></param>
    public StrTokenLiteral(string payload) => Payload = payload;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IStrTokenLiteral valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other) => Equals(other, StringComparison.CurrentCulture);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrTokenLiteral);

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
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.NotNullNotEmpty(trim: false);
    }
    string _Payload = string.Empty;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer) => this;
}