namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
public interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Determines whether this instance is equal to the other given one, or not.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    bool Equals(IParameter? other, bool caseSensitiveNames);

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The captured value of this parameter.
    /// </summary>
    object? Value { get; }
}