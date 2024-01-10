namespace Yotei.ORM.Internal;

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
    [SuppressMessage("", "IDE0290")]
    public TokenLiteral(string value) => Value = value.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Value;

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => null;
}