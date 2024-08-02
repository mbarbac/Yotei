namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null string value.
/// </summary>
public interface IStrTokenText : IStrToken
{
    /// <summary>
    /// The actual string carried by this instance.
    /// </summary>
    new string Payload { get; }
}