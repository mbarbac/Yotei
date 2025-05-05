using IHost = Yotei.ORM.Records.IIdentifierTags;
using THost = Yotei.ORM.Records.Code.IdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;
using TItem = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<IItem>]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class IdentifierTags : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public IdentifierTags(bool sensitive) => Items = new(sensitive);

    /// <summary>
    /// Initializes a new instance with the tags in the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierTags(THost source) : this(source.CaseSensitiveTags) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = other[i];
            if (!item.Equals(temp)) return false;
        }

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
        code = HashCode.Combine(code, CaseSensitiveTags);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    protected override Builder Items { get; }

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public override Builder GetBuilder() => Items.Clone();
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public bool CaseSensitiveTags => Items.CaseSensitiveTags;

    /// <inheritdoc/>
    public IEnumerable<string> Names => Items.Names;

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range) => Items.ContainsAny(range);

    /// <inheritdoc/>
    public int IndexOf(string name) => Items.IndexOf(name);

    /// <inheritdoc/>
    public int IndexOfAny(IEnumerable<string> range) => Items.IndexOfAny(range);

    /// <inheritdoc/>
    public int LastIndexOfAny(IEnumerable<string> range) => Items.LastIndexOfAny(range);

    /// <inheritdoc/>
    public List<int> IndexesOfAny(IEnumerable<string> range) => Items.IndexesOfAny(range);

    /// <inheritdoc/>
    public IHost Remove(string name)
    {
        var builder = GetBuilder();
        var done = builder.Remove(name);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IHost RemoveAll(IEnumerable<string> range)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAll(range);
        return done > 0 ? builder.ToInstance() : this;
    }
}