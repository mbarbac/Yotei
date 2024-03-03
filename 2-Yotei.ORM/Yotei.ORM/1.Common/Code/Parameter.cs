namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameter"/>
[WithGenerator]
public sealed partial class Parameter : IParameter
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
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Parameter(Parameter source)
    {
        Name = source.Name;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <inheritdoc/>
    public string Name { get; private set; }

    /// <inheritdoc/>
    public object? Value { get; private set; }
}