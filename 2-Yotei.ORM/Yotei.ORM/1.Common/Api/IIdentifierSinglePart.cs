namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary single-part database identifier.
/// </summary>
public interface IIdentifierSinglePart : IIdentifier
{
    /// <summary>
    /// The unwrapped value (aka: without the engine terminators) carried by this identifier, or
    /// null if it represents an empty or missed one.
    /// </summary>
    string? UnwrappedValue { get; }
}