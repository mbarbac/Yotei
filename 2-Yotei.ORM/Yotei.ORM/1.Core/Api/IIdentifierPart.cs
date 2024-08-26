namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface IIdentifierPart : IIdentifier
{
    /// <summary>
    /// The unwrapped value carried by this identifier, without the engine terminators, if any,
    /// or null if it represents an empty or missed one.
    /// </summary>
    string? UnwrappedValue { get; }
}