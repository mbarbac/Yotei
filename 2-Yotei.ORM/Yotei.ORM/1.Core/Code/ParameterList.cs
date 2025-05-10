using IHost = Yotei.ORM.IParameterList;
using THost = Yotei.ORM.Code.ParameterList;
using TItem = Yotei.ORM.Code.Parameter;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>]
[DebuggerDisplay("Items.{ToDebugString(5)}")]
public partial class ParameterList : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="capacity"></param>
    public ParameterList(IEngine engine, int capacity) => Items = new(engine, capacity);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, IItem item) : this(engine) => Items.Add(item);

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
    protected ParameterList(THost source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    protected override Builder Items { get; }

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public override Builder GetBuilder() => Items.Clone();
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(valid.Engine)) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item is TItem temp
                ? temp.Equals(valid[i], Engine.CaseSensitiveNames)
                : item.Equals(valid[i]);

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
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc cref="IHost.AddNew(object?, out IItem)"/>
    public THost AddNew(object? value, out IItem item)
    {
        var builder = GetBuilder();
        var done = builder.AddNew(value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.AddNew(object? value, out IItem item) => AddNew(value, out item);

    /// <inheritdoc cref="IHost.InsertNew(int, object?, out IItem)"/>
    public THost InsertNew(int index, object? value, out IItem item)
    {
        var builder = GetBuilder();
        var done = builder.InsertNew(index, value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.InsertNew(int index, object? value, out IItem item) => InsertNew(index, value, out item);
}