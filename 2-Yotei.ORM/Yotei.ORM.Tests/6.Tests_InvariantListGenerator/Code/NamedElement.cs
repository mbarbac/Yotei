namespace Yotei.ORM.Tests.InvariantListGenerator;

// ========================================================
/// <inheritdoc cref="IElement"/>
public partial class NamedElement : IElement
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
    /// Determines if this instance can be considered equal to the other given name, using the
    /// given comparison mode to compare their respective names.
    /// </summary>
    public virtual bool Equals(IElement? other, bool sensitive)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not NamedElement valid) return false;

        return string.Compare(Name, valid.Name, !sensitive) == 0;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IElement? other) => Equals(other, sensitive: true);

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
    /// The name carried by this element.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;
}