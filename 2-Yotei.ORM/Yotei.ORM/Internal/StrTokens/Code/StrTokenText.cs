namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenText"/>
public class StrTokenText : IStrTokenText
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static StrTokenText Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public StrTokenText() => Payload = string.Empty;

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenText(string value) => Payload = value ?? string.Empty;

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
    public IStrToken Tokenize(Func<string, IStrToken> tokenizer)
    {
        tokenizer.ThrowWhenNull();
        return tokenizer(Payload);
    }
}