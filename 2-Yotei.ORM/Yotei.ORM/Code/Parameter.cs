namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameter"/>
public sealed class Parameter : IParameter
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public Parameter(string name, object? value)
    {
        Name = name.NotNullNotEmpty();
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public object? Value { get; }
}