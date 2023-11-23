namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a part in a database identifier.
/// <br/> Values cannot contain unwrapped dots or spaces.
/// </summary>
public interface IIdentifierPart : IEquatable<IIdentifierPart>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or null if it represents an empty or missed one. If
    /// not, the value is wrapped with the engine terminators, if they are used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// The unwrapped value carried by this instance, or null if it represents an empty or missed
    /// one.
    /// </summary>
    string? UnwrappedValue { get; }
}