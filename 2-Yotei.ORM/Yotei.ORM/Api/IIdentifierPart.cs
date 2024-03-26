namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary single-part database identifier.
/// </summary>
public interface IIdentifierPart : IIdentifier
{
    /// <summary>
    /// The unwrapped value carried by this identifier, or null if it represents an empty or
    /// missed one. Unwrapped values are not wrapped by the engine's terminators.
    /// </summary>
    string? UnwrappedValue { get; }
}