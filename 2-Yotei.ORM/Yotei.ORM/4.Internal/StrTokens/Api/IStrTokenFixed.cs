namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary not-null string value that cannot be tokenized
/// any further. Values consisting in empty strings are accepted and used in special cases as
/// enforced separators.
/// </summary>
public interface IStrTokenFixed : IStrToken
{
    /// <summary>
    /// The actual string carried by this instance.
    /// </summary>
    new string Payload { get; }
}