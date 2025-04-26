using IHost = Yotei.ORM.Tools.Code.Templates.IElement;
using THost = Yotei.ORM.Tools.Code.Templates.Element;

namespace Yotei.ORM.Tools.Code.Templates;

// ========================================================
/// <inheritdoc cref="IHost"/>
public class Element : IHost
{
    /// <summary>
    /// Initializes a new instance with the given name.
    /// </summary>
    /// <param name="name"></param>
    public Element(string name) => Name = name.NotNullNotEmpty();

    /// <inheritdoc/>
    public string Name { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other) => Equals(other, caseSensitiveNames: true);

    /// <summary>
    /// Indicates whether this object is equal to another object of the same type, using the
    /// given comparison.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    public bool Equals(IHost? other, bool caseSensitiveNames)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        if (string.Compare(Name, valid.Name, !caseSensitiveNames) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        return code;
    }
}