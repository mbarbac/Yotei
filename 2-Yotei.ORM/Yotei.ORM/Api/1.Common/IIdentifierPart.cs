namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a part in an arbitrary identifier.
/// </summary>
public interface IIdentifierPart
{
    /// <summary>
    /// The value carried by this instace, or null if it represents an empty or missed one. If
    /// not null, it may be wrapped with the terminator pairs of the associated engine.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// The unwrapped value carried by this instace, or null if it represents an empty or missed
    /// identifier. Unwrapped values do not carry the terminator pairs defined by the associated
    /// engine.
    /// </summary>
    string? UnwrappedValue { get; }
}