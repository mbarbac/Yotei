namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <inheritdoc cref="IElement"/>
public class Element : IElement
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public Element(string name) => Name = name;

    /// <inheritdoc/>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is equal to the other given one, using the given comparison
    /// mode.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public virtual bool Equals(IElement? other, bool caseSensitive)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not Element valid) return false;

        if (string.Compare(Name, valid.Name, !caseSensitive) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IElement? other) => Equals(other, caseSensitive: true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IElement);

    public static bool operator ==(Element? host, IElement? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Element? host, IElement? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The name of this element.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;
}