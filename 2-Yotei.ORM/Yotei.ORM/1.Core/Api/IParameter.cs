namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// Determines whether the current object can be considered equal to another object of the
    /// same type, using the given comparison setting to compare their names.
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