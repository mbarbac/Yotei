using THost = Yotei.ORM.Code.ParameterList;
using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class ParameterList : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Items = new Builder(engine);

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
            var same = item.Equals(temp, Engine.CaseSensitiveNames);

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc/>
    public virtual IHost AddNew(object? value, out IItem item)
    {
        var builder = CreateBuilder();
        var done = builder.AddNew(value, out item);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost InsertNew(int index, object? value, out IItem item)
    {
        var builder = CreateBuilder();
        var done = builder.InsertNew(index, value, out item);
        return done > 0 ? builder.CreateInstance() : this;
    }
}
