namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameter"/>
[InheritWiths]
public partial class Parameter : IParameter
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
    protected Parameter(Parameter source)
    {
        source.ThrowWhenNull();

        Name = source.Name;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IParameter? other, bool caseSensitive)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (string.Compare(Name, other.Name, !caseSensitive) != 0) return false;
        if (!CompareValues()) return false;
        return true;

        bool CompareValues() // Use 'is' instead of '=='...
        {
            if (Value is null && other.Value is null) return true;
            if (Value is null || other.Value is null) return false;

            return Value.Equals(other.Value);
        }
    }

    /// <inheritdoc/>
    public virtual bool Equals(IParameter? other) => Equals(other, caseSensitive: true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IParameter);

    public static bool operator ==(Parameter? x, IParameter? y) // Use 'is' instead of '=='...
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return x.Equals(y);
    }

    public static bool operator !=(Parameter? host, IParameter? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <inheritdoc/>
    public object? Value { get; init; }
}