namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null string value.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class StrTokenText : StrToken
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
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenText(string value) => Payload = value.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc/>
    public override string Payload { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override StrToken Reduce(StringComparison comparison) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override StrToken TokenizeWith(
        Func<string, StrToken> tokenizer) => tokenizer.ThrowWhenNull()(Payload);
}