namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier, including empty or missed ones.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IIdentifierUnit : IIdentifier
{
    /// <summary>
    /// The raw value carried by this instance (without any engine terminators), or '<c>null</c>'
    /// if it represents an empty or missed identifier.
    /// </summary>
    string? RawValue { get; }
}