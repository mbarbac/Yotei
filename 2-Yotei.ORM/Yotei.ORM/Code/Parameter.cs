namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameter"/>
/// <param name="name"></param>
/// <param name="value"></param>
public sealed class Parameter(string name, object? value) : IParameter
{
    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <summary>
    /// Determines whether this instance is equal to the other given one, or not.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    public bool Equals(IParameter? other, bool caseSensitiveNames)
    {
        if (other is null) return false;

        if (string.Compare(Name, other.Name, !caseSensitiveNames) != 0) return false;
        if (!Value.EqualsEx(other.Value)) return false;

        return true;
    }

    /// <inheritdoc/>
    public bool Equals(IParameter? other) => Equals(other, true);
    public override bool Equals(object? obj) => Equals(obj as IParameter);
    public static bool operator ==(Parameter x, IParameter y) => x is not null && x.Equals(y);
    public static bool operator !=(Parameter x, IParameter y) => !(x == y);
    public override int GetHashCode() => HashCode.Combine(Name, Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Name { get; } = name.NotNullNotEmpty();

    /// <inheritdoc/>
    public object? Value { get; } = value;
}