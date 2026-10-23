using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaElement;
using IHost = Yotei.ORM.Records.ISchemaDescriptor;
using THost = Yotei.ORM.Records.Code.SchemaDescriptor;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class SchemaDescriptor : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaDescriptor(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaDescriptor(IEngine engine, IEnumerable<IItem> range) => throw null;

    /// <summary>
    /// Initializes a new instance of the Builder class by copying values from an existing instance.
    /// </summary>
    /// <param name="other">The Builder instance to copy values from.</param>
    protected SchemaDescriptor(THost other) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Builder Items => (Builder)base.Items;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    protected override Builder CreateItems() => new(Engine);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IHost.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(IHost? other) => throw null;
    /*{
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(other.Engine)) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = valid[i];
            var same = item.Equals(temp, Engine.IgnoreCase);
            if (!same) return false;
        }

        return true;
    }*/

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><inheritdoc/></returns>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode() => throw null;
    /*{
        var code = Engine.GetHashCode();
        code = HashCode.Combine(code, Items);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool Contains(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int IndexOf(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int LastIndexOf(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string identifier) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public List<int> Match(string? specs) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    public List<int> Match(string? specs, out IItem? unique) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns><inheritdoc/></returns>
    public virtual IHost Remove(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns><inheritdoc/></returns>
    public virtual IHost RemoveLast(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns><inheritdoc/></returns>
    public virtual IHost RemoveAll(string identifier) => throw null;
}