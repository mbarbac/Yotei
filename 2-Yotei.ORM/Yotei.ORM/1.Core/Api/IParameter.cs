namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value captured by this parameter.
    /// </summary>
    object? Value { get; }
}