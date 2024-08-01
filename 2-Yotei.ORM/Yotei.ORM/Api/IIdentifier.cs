namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier
{
    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    string? Value { get; }
}