namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
public interface IIdentifierPart : IIdentifier
{
    /// <summary>
    /// Determines if this instance can be considered equal to the other given one, using the
    /// given comparison mode to compare their respective values.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    bool Equals(IIdentifier? other, bool caseSensitiveNames);

    /// <summary>
    /// The unwrapped value carried by this identifier, without any terminators, or null if it
    /// represents an empty or missed one.
    /// </summary>
    string? UnwrappedValue { get; }
}