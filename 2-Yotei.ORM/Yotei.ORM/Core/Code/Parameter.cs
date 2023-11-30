namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameter"/>
/// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; }
}