namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a parameter in a command.
/// </summary>
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Determines if this instance can be considered as equal to the other given one, using
    /// the given setting for comparing their respective names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    bool Equals(IParameter? other, bool caseSensitive);

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [WithGenerator] string Name { get; }

    /// <summary>
    /// The value carried by this parameter.
    /// </summary>
    [WithGenerator] object? Value { get; }
}