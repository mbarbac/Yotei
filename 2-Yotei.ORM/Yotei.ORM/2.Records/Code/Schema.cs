using IHost = Yotei.ORM.ISchema;
using IItem = Yotei.ORM.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<TKey, IItem>]
public partial class Schema : IHost
{
    /// <inheritdoc/>
    protected override Builder Items { get; }
    protected virtual Builder OnInitialize(IEngine engine) => new(engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine) => Items = OnInitialize(engine);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Schema(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Schema(Schema source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other) => throw null;

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string identifier) => throw null;

    /// <inheritdoc/>
    public int IndexOf(string identifier) => throw null;

    /// <inheritdoc/>
    public int LastIndexOf(string identifier) => throw null;

    /// <inheritdoc/>
    public List<int> IndexesOf(string identifier) => throw null;

    /// <inheritdoc/>
    public List<int> Match(string? specs) => throw null;

    /// <inheritdoc/>
    public List<int> Match(string? specs, out IItem? unique) => throw null;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual Schema Remove(string identifier) => throw null;
    IHost IHost.Remove(string identifier) => throw null;

    /// <inheritdoc/>
    public virtual Schema RemoveLast(string identifier) => throw null;
    IHost IHost.RemoveLast(string identifier) => throw null;

    /// <inheritdoc/>
    public virtual Schema RemoveAll(string identifier) => throw null;
    IHost IHost.RemoveAll(string identifier) => throw null;
}