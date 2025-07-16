using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<TKey, IItem>]
public partial class Schema : IHost
{
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
    /// <param name="sensitive"></param>
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
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

    public static bool operator ==(Schema? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Schema? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.CreateBuilder"/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string identifier) => Items.Contains(identifier);

    /// <inheritdoc/>
    public int IndexOf(string identifier) => Items.IndexOf(identifier);

    /// <inheritdoc/>
    public List<int> Match(string? specs) => Items.Match(specs);

    /// <inheritdoc/>
    public List<int> Match(string? specs, out IItem? unique) => Items.Match(specs, out unique);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual Schema Remove(string identifier)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(identifier);
        return done ? builder.CreateInstance() : this;
    }
    IHost IHost.Remove(string identifier) => Remove(identifier);
}