namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Determines if this identifier matches the given specifications. Comparison is performed
    /// by comparing parts from right to left, where any empty or null one is taken as an implicit
    /// match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);
}