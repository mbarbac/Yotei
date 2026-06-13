namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Cloneable(ReturnType = typeof(IParameter))]
[InheritsWith(ReturnType = typeof(IParameter))]
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
    /// <param name="other"></param>
    protected Parameter(Parameter other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Name = other.Name;
        Value = other.Value.TryClone();
    }

    public override string ToString() => Value is null
        ? $"{Name}=NULL"
        : $"{Name}='{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <param name="ignoreNameCase"></param>
    /// <returns></returns>
    public virtual bool Equals(IParameter? other, bool ignoreNameCase)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (string.Compare(Name, other.Name, ignoreNameCase) != 0) return false;
        if (!Value.EqualsEx(other.Value)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IParameter? other) => Equals(other, false);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IParameter);

    public static bool operator ==(Parameter? host, IParameter? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Parameter? host, IParameter? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; init => field = value.NotNullNotEmpty(trim: true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; init; }
}