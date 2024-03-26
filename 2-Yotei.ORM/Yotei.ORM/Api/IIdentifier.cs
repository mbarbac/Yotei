namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier : IEquatable<IIdentifier>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    string? Value { get; }
}