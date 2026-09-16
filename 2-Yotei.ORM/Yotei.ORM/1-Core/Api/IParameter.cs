namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Determines if this instance shall be considered equal to the other given one, using
    /// the given <paramref name="ignoreNameCase"/> value to compare the names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="ignoreNameCase"></param>
    /// <returns></returns>
    bool Equals(IParameter? other, bool ignoreNameCase);

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value carried by this instance.
    /// </summary>
    [With] object? Value { get; }
}