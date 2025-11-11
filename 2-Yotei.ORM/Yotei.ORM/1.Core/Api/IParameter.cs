namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type, using
    /// the given comparison mode when comparing their respective names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    bool Equals(IParameter? other, bool caseSensitiveNames);

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value captured by this parameter.
    /// </summary>
    [With] object? Value { get; }
}