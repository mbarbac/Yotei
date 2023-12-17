namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a parameter given to a command.
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