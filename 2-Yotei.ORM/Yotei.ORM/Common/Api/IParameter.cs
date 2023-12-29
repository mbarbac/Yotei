namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a command parameter.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The captured value of this parameter.
    /// </summary>
    object? Value { get; }
}