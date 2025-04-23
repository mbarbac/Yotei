namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null and not-empty keyword value that will
/// be not tokenized any further. Keywords are trimmed before used.
/// </summary>
public interface IStrTokenKeyword : IStrToken
{
    /// <summary>
    /// The actual not-null immutable trimmed keyword payload carried by this instance.
    /// </summary>
    new string Payload { get; }
}