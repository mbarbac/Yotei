namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier
{
    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// <br/> If not null, the value or its parts may be wrapped with the terminators of the
    /// engine used to generate it.
    /// </summary>
    string? Value { get; }
}