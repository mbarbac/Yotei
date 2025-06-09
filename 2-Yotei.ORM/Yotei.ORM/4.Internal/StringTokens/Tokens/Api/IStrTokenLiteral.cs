namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a string token that carries an arbitrary not-null and not-empty string value that
/// will not be tokenized any further.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
public interface IStrTokenLiteral: IStrToken
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