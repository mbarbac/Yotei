namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a parameter in a command.
/// </summary>
[WithGenerator("(source)+@")]
public partial class Parameter : IParameter
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Parameter() { }

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
    protected Parameter(Parameter source)
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