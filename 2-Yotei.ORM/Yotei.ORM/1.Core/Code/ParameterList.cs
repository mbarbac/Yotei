using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<TKey, IItem>]
public partial class ParameterList : IHost
{
    protected override Builder Items { get; }
    protected ParameterList(Builder items) => Items = items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) : this(new Builder(engine)) { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ParameterList(ParameterList source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(valid.Engine)) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            if (!item.Equals(temp, Engine.CaseSensitiveNames)) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(ParameterList? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(ParameterList? host, IHost? item) => !(host == item);

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
    public IEngine Engine
    {
        get => Items.Engine;
        init => Items.Engine = value;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc/>
    public virtual ParameterList AddNew(object? value, out IItem item)
    {
        var clone = Clone();
        var done = clone.Items.AddNew(value, out item);
        return done > 0 ? clone : this;
    }
    IHost IHost.AddNew(object? value, out IItem item) => AddNew(value, out item);

    /// <inheritdoc/>
    public virtual ParameterList InsertNew(int index, object? value, out IItem item)
    {
        var clone = Clone();
        var done = clone.Items.InsertNew(index, value, out item);
        return done > 0 ? clone : this;
    }
    IHost IHost.InsertNew(int index, object? value, out IItem item) => InsertNew(index, value, out item);
}