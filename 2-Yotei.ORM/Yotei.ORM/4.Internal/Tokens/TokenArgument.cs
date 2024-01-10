namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the argument of a dynamic lambda expression. Instances of this class are considered
/// translation artifacts with no database representation.
/// </summary>
public sealed class TokenArgument : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenArgument(string name) => Name = name.ValidateTokenName();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Name;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => this;
}