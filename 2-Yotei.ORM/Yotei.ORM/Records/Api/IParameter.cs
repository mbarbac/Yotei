namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an immutable parameter in a command.
/// </summary>
public partial interface IParameter
{
    /// <summary>
    /// The name of this parameter.
    /// </summary>
    [WithGenerator] string Name { get; }

    /// <summary>
    /// The value carried by this parameter.
    /// </summary>
    [WithGenerator] object? Value { get; }
}