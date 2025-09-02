namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IIdentifierUnit : IIdentifier
{
    /// <summary>
    /// Determines if this instance can be considered equal to the other given one, using the
    /// given comparison mode to compare their respective values.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    bool Equals(IIdentifier? other, bool caseSensitive);

    /// <summary>
    /// The raw or unwrapped value carried by this identifier, without any terminators, or null
    /// if it represents an empty or missed one.
    /// </summary>
    string? RawValue { get; }
}