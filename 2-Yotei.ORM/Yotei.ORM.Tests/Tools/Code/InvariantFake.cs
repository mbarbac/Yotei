namespace Yotei.ORM.Tools.Code;

// ========================================================
public class InvariantFake(string name) : IInvariantFake
{
    public override string ToString() => Name ?? string.Empty;
    public string Name { get; set; } = name;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IInvariantFake? other)
    {
        if (other is null) return false;

        if (Name != other.Name) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as InvariantFake);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Name.GetHashCode();
}