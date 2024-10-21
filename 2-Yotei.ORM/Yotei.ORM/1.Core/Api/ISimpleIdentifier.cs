namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// </summary>
public interface ISimpleIdentifier : IIdentifier
{
    /// <summary>
    /// The unwrapped value carried by this identifier, or null if it represents an empty or
    /// missed one.
    /// </summary>
    string? UnwrappedValue { get; }
}