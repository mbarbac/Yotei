namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a parameter in a command.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value carried by this parameter.
    /// </summary>
    object? Value { get; }
}