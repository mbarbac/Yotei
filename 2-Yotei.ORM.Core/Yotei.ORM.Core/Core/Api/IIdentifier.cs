namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// The immutable object that represents a database identifier.
/// </summary>
public interface IIdentifier
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it is an empty or missed one.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Reduces this instance to a simpler one, if such is possible.
    /// </summary>
    /// <returns></returns>
    IIdentifier Reduce();

    /// <summary>
    /// Determines if this instance matches the given target one, or not. Matching is performed
    /// from right to left, comparing the non-termnated value of each part where empty or missed
    /// ones are considered an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(IIdentifier target);
}