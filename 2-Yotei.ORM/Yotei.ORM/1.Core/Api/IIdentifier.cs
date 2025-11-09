namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier, including empty or missed ones.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IIdentifier : IEquatable<IIdentifier>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or '<c>null</c>' if it represents an empty or missed
    /// identifier.
    /// </summary>
    string? Value { get; }
}