namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// Represents a database identifier.
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
}