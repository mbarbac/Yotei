namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null and not-empty string value that will
/// be not tokenized any further.
/// </summary>
public interface IStrTokenLiteral : IStrToken
{
    /// <summary>
    /// The actual not-null immutable string payload carried by this instance.
    /// </summary>
    new string Payload { get; }
}