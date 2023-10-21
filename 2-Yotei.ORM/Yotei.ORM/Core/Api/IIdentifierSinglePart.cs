namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface IIdentifierSinglePart : IIdentifier
{
    /// <summary>
    /// The non-terminated value carried by this identifier, or null if it is an empty or missed
    /// one. If the underlying engine does not use terminators, then this value is the same as
    /// the <see cref="Value"/> one.
    /// </summary>
    string? NonTerminatedValue { get; }
}