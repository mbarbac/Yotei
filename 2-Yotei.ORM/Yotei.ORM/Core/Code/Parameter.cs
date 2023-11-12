namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameter"/>
/// </summary>
[WithGenerator(PreventVirtual = true)]
public sealed partial class Parameter : IParameter
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public Parameter(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Parameter(Parameter source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Name = source.Name;
        Value = source.Value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <summary>
    /// The name of this parameter.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <summary>
    /// The value carried by this parameter.
    /// </summary>
    public object? Value { get; init; }
}