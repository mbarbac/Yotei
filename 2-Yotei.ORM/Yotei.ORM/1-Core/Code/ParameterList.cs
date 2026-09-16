using TKey = string;
using IItem = Yotei.ORM.IParameter;
using IHost = Yotei.ORM.IParameterList;
using THost = Yotei.ORM.Code.ParameterList;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class ParameterList : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new intance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(IEngine engine, IEnumerable<IItem> range)
        : this(engine)
        => Items.AddRange(range.ThrowWhenNull());

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected ParameterList(THost other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Engine = other.Engine;
        Items.AddRange(other);
    }

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
    public virtual bool Equals(IHost? other)
    {
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
    }

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
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        code = HashCode.Combine(code, Items);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual string NextName() => Items.NextName();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual IHost AddNew(object? value, out IItem item)
    {
        var builder = ToBuilder();
        var done = builder.AddNew(value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual IHost InsertNew(int index, object? value, out IItem item)
    {
        var builder = ToBuilder();
        var done = builder.InsertNew(index, value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
}