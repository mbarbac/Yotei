namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null string value.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
public interface IStrTokenText : IStrToken
{
    /// <summary>
    /// Determines if this instance shall be considered equal to the other given one, using the
    /// given string comparison mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    bool Equals(IStrToken? other, StringComparison comparison);

    /// <summary>
    /// The actual not-null string payload carried by this instance.
    /// </summary>
    new string Payload { get; }
}