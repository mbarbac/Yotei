namespace Yotei.ORM.Tests.Templates;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantFake"/>
/// </summary>
public class InvariantFake(string name) : IInvariantFake
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IInvariantFake? other)
        => other is not null
        && Name == other.Name;

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name ?? string.Empty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; } = name;
}