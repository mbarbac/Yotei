namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null string value.
/// </summary>
public interface IStrTokenText : IStrToken
{
    /// <summary>
    /// A common shared empty instance.
    /// </summary>
    static abstract IStrTokenText Empty { get; }

    /// <summary>
    /// The actual not-null and not-empty string payload carried by this instance.
    /// </summary>
    new string Payload { get; }
}