namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameter"/>
/// </summary>
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
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public bool Equals(IParameter? other, bool caseSensitive)
    {
        if (other is null) return false;

        if (string.Compare(Name, other.Name, !caseSensitive) != 0) return false;

        if (Value is null && other.Value is null) return true;
        if (Value is not null && Value.Equals(other.Value)) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IParameter? other) => Equals(other, true);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IParameter);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = HashCode.Combine(Name);
        code = HashCode.Combine(code, Value);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; init; }
}