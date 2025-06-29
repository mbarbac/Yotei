namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <summary>
/// Represents an element in a collection.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
public interface IElement : IEquatable<IElement>
{
    /// <summary>
    /// Determines if this instance is equal to the other given one, using the given comparison
    /// mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    bool Equals(IElement? other, bool caseSensitive);

    /// <summary>
    /// The name of this element.
    /// </summary>
    string Name { get; }
}