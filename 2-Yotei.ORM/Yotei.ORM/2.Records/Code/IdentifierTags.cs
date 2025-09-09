using THost = Yotei.ORM.Records.Code.IdentifierTags;
using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class IdentifierTags : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public IdentifierTags(bool sensitive) => Items = new Builder(sensitive);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierTags(
        THost source) : this(source.CaseSensitiveTags) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = other[i];
            var same = item.Equals(temp);

            if (!same) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = CaseSensitiveTags.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags => Items.CaseSensitiveTags;

    /// <inheritdoc/>
    public IEnumerable<string> TagNames => Items.TagNames;

    /// <inheritdoc/>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

    /// <inheritdoc/>
    public int IndexOf(string name) => Items.IndexOf(name);

    /// <inheritdoc/>
    public int IndexOf(IEnumerable<string> range) => Items.IndexOf(range);

    /// <inheritdoc/>
    public List<int> IndexesOf(IEnumerable<string> range) => Items.IndexesOf(range);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost Remove(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(range);
        return done > 0 ? builder.CreateInstance() : this;
    }
}