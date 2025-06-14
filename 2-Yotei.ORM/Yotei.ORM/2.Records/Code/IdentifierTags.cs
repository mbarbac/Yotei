using IHost = Yotei.ORM.IIdentifierTags;
using IItem = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IItem>]
public partial class IdentifierTags : IHost
{
    /// <inheritdoc/>
    protected override Builder Items { get; }
    protected virtual Builder OnInitialize(bool sensitiveTags) => new(sensitiveTags);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitiveTags"></param>
    public IdentifierTags(bool sensitiveTags) => Items = OnInitialize(sensitiveTags);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="sensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool sensitiveTags, IEnumerable<IItem> range) : this(sensitiveTags) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierTags(IdentifierTags source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(other[i]);
            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(IdentifierTags? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(IdentifierTags? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveTags);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags => Items.CaseSensitiveTags;

    /// <inheritdoc/>
    public IEnumerable<string> Names => Items.Names;

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Items.IndexOf(name) >= 0;

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

    /// <inheritdoc/>
    public int IndexOf(string name) => Items.IndexOf(name);

    /// <inheritdoc/>
    public int IndexOf(IEnumerable<string> range) => Items.IndexOf(range);

    /// <inheritdoc/>
    public int LastIndexOf(IEnumerable<string> range) => Items.LastIndexOf(range);

    /// <inheritdoc/>
    public List<int> IndexesOf(IEnumerable<string> range) => Items.IndexesOf(range);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual IdentifierTags Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.Remove(string name) => Remove(name);

    /// <inheritdoc/>
    public virtual IdentifierTags Remove(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(range);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.Remove(IEnumerable<string> range) => Remove(range);
}