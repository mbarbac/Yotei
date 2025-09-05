namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
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

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance matches the given specifications, or not.
    /// <br/> Matching is performed by comparing the identifier parts with the given specs, from
    /// right to left, where an empty or null specification is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);
}