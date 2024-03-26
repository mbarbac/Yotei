namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents the argument of a dynamic lambda expression.
/// <br/> Instances of this class are considered translation artifacts, with no representation
/// in a database command.
/// </summary>
public sealed class TokenArgument : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public TokenArgument(string name) => Name = name.ValidateTokenName();

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() => this;
}