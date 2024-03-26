namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary literal in a database expression that, by convention, will never be
/// captured as an argument.
/// </summary>
public sealed class TokenLiteral : Token
{
    /// <summary>
    /// Represents an empty literal.
    /// </summary>
    public static TokenLiteral Empty { get; } = new(string.Empty);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public TokenLiteral(string value) => Value = value.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() => null;
}