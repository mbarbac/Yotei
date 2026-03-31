namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IElement"/>
/// </summary>
public class NamedElement : IElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public NamedElement(string name) => Name = name;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Equals(IElement?)"/>
    /// </summary>
    /// <param name="other"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public bool Equals(IElement? other, bool ignoreCase)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not NamedElement valid) return false;

        return string.Compare(Name, valid.Name, ignoreCase) == 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IElement? other) => Equals(other, false);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IElement);

    public static bool operator ==(NamedElement? host, IElement? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(NamedElement? host, IElement? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Name.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The name carried by this instance.
    /// </summary>
    public string Name { get; init => field = value.NotNullNotEmpty(trim: true); }
}