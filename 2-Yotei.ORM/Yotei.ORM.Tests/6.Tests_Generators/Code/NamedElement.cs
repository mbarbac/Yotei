namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <inheritdoc cref="INamedElement"/>
public class NamedElement : INamedElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public NamedElement(string name) => Name = name;

    /// <inheritdoc/>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IElement? other, bool caseSensitive)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not NamedElement valid) return false;

        if (string.Compare(Name, valid.Name, !caseSensitive) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IElement? other) => Equals(other, caseSensitive: true);

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

    /// <inheritdoc/>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;
}