namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IStrTokenText"/>
public class StrTokenText : IStrTokenText
{
    /// <summary>
    /// A common shared empty instance.
    /// </summary>
    public static StrTokenText Empty = new();

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IStrTokenText valid) return false;

        if (string.Compare(Payload, valid.Payload, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IStrToken? other) => Equals(other, StringComparison.CurrentCulture);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IStrTokenText);

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
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    string _Payload = string.Empty;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken TokenizeWith(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();
        return tokenizer(_Payload);
    }
}