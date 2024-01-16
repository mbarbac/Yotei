namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface IIdentifierPart
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier part, or null if it represents an empty or missed
    /// one. If not, its value is wrapped with the appropriate engine terminators.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// The unwrapped value carried by this identifier part, or null if it represents an empty or
    /// missed one. If not, its value is a raw literal not wrapped with any engine terminators.
    /// </summary>
    string? UnwrappedValue { get; }
}