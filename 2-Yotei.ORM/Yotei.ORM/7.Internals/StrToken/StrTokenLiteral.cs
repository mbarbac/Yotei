namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null and not-empyt literal value that
/// will be not tokenized any further.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class StrTokenLiteral : StrToken
{
    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenLiteral(string value) => Payload = value.NotNullNotEmpty(trim: false);

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc/>
    public override string Payload { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override StrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override StrToken TokenizeWith(Func<string, StrToken> tokenizer) => this;
}