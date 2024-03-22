namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameter"/>
/// <param name="name"></param>
/// <param name="value"></param>
public sealed class Parameter(string name, object? value) : IParameter
{
    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <inheritdoc/>
    public string Name { get; } = name.NotNullNotEmpty();

    /// <inheritdoc/>
    public object? Value { get; } = value;
}