using IHost = Yotei.ORM.Records.ISchema;
using THost = Yotei.ORM.Records.Code.Schema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>]
public partial class Schema : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Schema(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, IItem item) => throw null;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, IEnumerable<IItem> item) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Schema(THost source) => throw null;

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    protected override Builder Items => throw null;

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public override Builder GetBuilder() => throw null;
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other) => throw null;

    /*
     /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);

            if (!equal) return false;
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
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
     */

    // ----------------------------------------------------

    /// <inheritdoc/>
    public List<int> Match(string? specs) => throw null;

    /// <inheritdoc/>
    public List<int> Match(string? specs, out IItem? unique) => throw null;

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string identifier) => throw null;

    /// <inheritdoc/>
    public int IndexOf(string identifier) => throw null;

    /// <inheritdoc/>
    public int LastIndexOf(string identifier) => throw null;

    /// <inheritdoc/>
    public List<int> IndexesOf(string identifier) => throw null;

    // ------------------------------------------------

    /// <inheritdoc/>
    public IHost Remove(string identifier) => throw null;

    /// <inheritdoc/>
    public IHost RemoveLast(string identifier) => throw null;

    /// <inheritdoc/>
    public IHost RemoveAll(string identifier) => throw null;
}