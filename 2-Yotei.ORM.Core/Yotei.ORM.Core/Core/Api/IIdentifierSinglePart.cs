namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// The immutable object that represents a single-part database identifier.
/// </summary>
public interface IIdentifierSinglePart : IIdentifier
{
    /// <summary>
    /// The non-terminated value carried by this identifier, or null if it is an empty or missed
    /// one.
    /// </summary>
    string? NonTerminatedValue { get; }
}