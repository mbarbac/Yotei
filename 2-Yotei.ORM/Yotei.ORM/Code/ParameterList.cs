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