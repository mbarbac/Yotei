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

    /// <summary>
    /// Invoked to create the initial repository of contents of this instance.
    /// </summary>
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
    public virtual Builder CreateBuilder() => throw null;
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other) => throw null;

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
    public virtual IdentifierTags Remove(string name) => throw null;
    IHost IHost.Remove(string name) => Remove(name);

    /// <inheritdoc/>
    public virtual IdentifierTags Remove(IEnumerable<string> range) => throw null;
    IHost IHost.Remove(IEnumerable<string> range) => Remove(range);
}