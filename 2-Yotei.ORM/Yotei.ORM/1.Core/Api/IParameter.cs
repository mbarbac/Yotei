namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
public partial interface IParameter : IEquatable<IParameter>
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value captured by this parameter.
    /// </summary>
    [With] object? Value { get; }
}