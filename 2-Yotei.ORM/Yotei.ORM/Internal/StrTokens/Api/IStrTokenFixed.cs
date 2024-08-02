namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null and not-empty string value that shall
/// not be tokenized further. Values consisting of spaces are not considered empty ones.
/// </summary>
public interface IStrTokenFixed : IStrToken
{
    /// <summary>
    /// The actual string carried by this instance.
    /// </summary>
    new string Payload { get; }
}