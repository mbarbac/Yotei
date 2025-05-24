using IHost = Yotei.ORM.ISchema;
using THost = Yotei.ORM.Code.Schema;
using IItem = Yotei.ORM.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class Schema : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine, int capacity) => Items = new(engine, capacity);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, IItem item) => Items = new(engine, item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Schema(IEngine engine, IEnumerable<IItem> range) => Items = new(engine, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Schema(THost source) => Items = new(source.Engine, source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    protected override Builder Items { get; }

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public override Builder GetBuilder() => Items.Clone();
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
            if (!Items[i].EqualsEx(other[i])) return false;

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
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string identifier) => Items.Contains(identifier);

    /// <inheritdoc/>
    public int IndexOf(string identifier) => Items.IndexOf(identifier);

    /// <inheritdoc/>
    public int LastIndexOf(string identifier) => Items.LastIndexOf(identifier);

    /// <inheritdoc/>
    public List<int> IndexesOf(string identifier) => Items.IndexesOf(identifier);

    /// <inheritdoc/>
    public List<int> Match(string? specs) => Items.Match(specs);

    /// <inheritdoc/>
    public List<int> Match(string? specs, out IItem? unique) => Items.Match(specs, out unique);

    // ------------------------------------------------

    /// <inheritdoc/>
    public IHost Remove(string identifier)
    {
        var builder = GetBuilder();
        var done = builder.Remove(identifier);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IHost RemoveLast(string identifier)
    {
        var builder = GetBuilder();
        var done = builder.RemoveLast(identifier);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IHost RemoveAll(string identifier)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAll(identifier);
        return done ? builder.ToInstance() : this;
    }
}