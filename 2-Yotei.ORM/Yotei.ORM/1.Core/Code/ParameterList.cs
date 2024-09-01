namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterList"/>
[FrozenList<string, IParameter>]
public partial class ParameterList : IParameterList
{
    /// <inheritdoc/>
    public virtual ParameterListBuilder ToBuilder() => Items.Clone();
    IParameterListBuilder IParameterList.ToBuilder() => ToBuilder();

    protected override ParameterListBuilder Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) : base() => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, IParameter item) : this(engine) => Items.Add(item);

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
    ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source.Items);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IParameterList? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; ++i)
        {
            var source = this[i];
            var target = other[i];

            if (string.Compare(source.Name, target.Name, !Engine.CaseSensitiveNames) != 0) return false;
            if (!source.Value.EqualsEx(target.Value)) return false;
        }

        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IParameterList);
    public static bool operator ==(ParameterList x, IParameterList y) => x is not null && x.Equals(y);
    public static bool operator !=(ParameterList x, IParameterList y) => !(x == y);
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc/>
    public virtual ParameterList AddNew(object? value, out IParameter item)
    {
        var builder = Items.Clone();
        var done = builder.AddNew(value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IParameterList IParameterList.AddNew(
        object? value, out IParameter item) => AddNew(value, out item);

    /// <inheritdoc/>
    public virtual ParameterList InsertNew(int index, object? value, out IParameter item)
    {
        var builder = Items.Clone();
        var done = builder.InsertNew(index, value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IParameterList IParameterList.InsertNew(
        int index, object? value, out IParameter item) => InsertNew(index, value, out item);
}