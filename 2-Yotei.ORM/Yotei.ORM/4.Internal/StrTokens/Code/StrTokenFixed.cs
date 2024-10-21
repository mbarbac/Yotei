namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenFixed"/>
public class StrTokenFixed : IStrTokenFixed
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenFixed() => Payload = string.Empty;

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenFixed(string value) => Payload = value;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc/>
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.ThrowWhenNull();
    }
    string _Payload = default!;
    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;

    /// <inheritdoc/>
    public IStrToken Tokenize(Func<string, IStrToken> tokenizer) => this;
}