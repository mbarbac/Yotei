namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
public interface IIdentifierPart : IIdentifier
{
    /// <summary>
    /// The unwrapped value carried by this identifier, without any terminators, or null if it
    /// represents an empty or missed one.
    /// </summary>
    string? UnwrappedValue { get; }
}