namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterList"/>
[DebuggerDisplay("{Items.ToDebugString(5)}")]
[InvariantList<string, IParameter>]
public partial class ParameterList : IParameterList
{
    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(
        IEngine engine, IEnumerable<IParameter> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IParameterList? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(other[i], Engine.CaseSensitiveNames);
            if (!equal) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IParameterList);

    public static bool operator ==(ParameterList? host, IParameterList? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(ParameterList? host, IParameterList? item) => !(host == item);

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
    public IEngine Engine => Items.Engine;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc/>
    public virtual IParameterList AddNew(object? value, out IParameter item)
    {
        var clone = Clone();
        var done = clone.Items.AddNew(value, out item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IParameterList InsertNew(int index, object? value, out IParameter item)
    {
        var clone = Clone();
        var done = clone.Items.InsertNew(index, value, out item);
        return done > 0 ? clone : this;
    }
}