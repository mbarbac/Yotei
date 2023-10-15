namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface IIdentifierSinglePart : IIdentifier
{
    /// <summary>
    /// The non-terminated value carried by this identifier, or null if it is an empty or missed
    /// one. The value of this property is the same as the <see cref="Value"/> one if terminators
    /// are not used by the underlying engine.
    /// </summary>
    string? NonTerminatedValue { get; }
}