namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface IIdentifierPart : IIdentifier
{
    /// <summary>
    /// The unwrapped value carried by this identifier, without any terminators, or null if it
    /// represents an empty or missed one.
    /// </summary>
    string? UnWrappedValue { get; }
}