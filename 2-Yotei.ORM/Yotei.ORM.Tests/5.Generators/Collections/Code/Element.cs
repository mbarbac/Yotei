using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using IHost = Yotei.ORM.Tests.Generators.IElement;
using THost = Yotei.ORM.Tests.Generators.Element;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <inheritdoc cref="IHost"/>
public class Element : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public Element(string name) => Name = name;

    /// <inheritdoc/>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (string.Compare(Name, valid.Name, comparison) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other) => Equals(other, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

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