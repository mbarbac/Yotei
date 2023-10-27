namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an immutable database identifier.
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

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler one, if such is possible.
    /// </summary>
    /// <returns></returns>
    IIdentifier Reduce();

    /// <summary>
    /// Determines if this instance matches the filters expressed by the given target one, where
    /// matching is performed from right to left using the non-terminated values, and any null,
    /// empty, or missed filter is considered as an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(IIdentifier target);
}