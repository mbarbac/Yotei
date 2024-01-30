namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
public partial interface IParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [WithGenerator]
    string Name { get; }

    /// <summary>
    /// The captured value of this parameter.
    /// </summary>
    [WithGenerator]
    object? Value { get; }
}