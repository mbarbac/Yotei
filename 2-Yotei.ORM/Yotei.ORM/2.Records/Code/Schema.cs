using THost = Yotei.ORM.Records.Code.Schema;
using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class Schema : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine) => Items = new(engine);

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
    protected Schema(THost source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
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
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public bool Contains(string identifier) => Items.Contains(identifier);

    /// <inheritdoc/>
    public int IndexOf(string identifier) => Items.IndexOf(identifier);

    /// <inheritdoc/>
    public List<int> Match(string? specs) => Items.Match(specs);

    /// <inheritdoc/>
    public List<int> Match(string? specs, out IItem? unique) => Items.Match(specs, out unique);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost Remove(string identifier)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(identifier);
        return done ? builder.CreateInstance() : this;
    }
}