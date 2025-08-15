namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Determines if this instance is equal to the other given one, using the given comparison
    /// mode to compare their names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    bool Equals(IParameter? other, bool caseSensitive);

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value captured by this parameter.
    /// </summary>
    [With] object? Value { get; }
}