namespace Yotei.ORM.Generators.Invariant.Tests;

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

    /// <inheritdoc/>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type, using
    /// the given comparison mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public virtual bool Equals(IElement? other, bool caseSensitive)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not NamedElement valid) return false;

        return string.Compare(Name, valid.Name, !caseSensitive) == 0;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type, using
    /// case sensitive name comparison.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IElement? other) => Equals(other, true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IElement);

    public static bool operator ==(NamedElement? host, IElement? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(NamedElement? host, IElement? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The name of this element.
    /// </summary>
    public string Name { get; init => field = value.NotNullNotEmpty(true); }
}