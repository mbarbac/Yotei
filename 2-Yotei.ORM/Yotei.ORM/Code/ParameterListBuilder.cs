namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterListBuilder"/>
[Cloneable]
public partial class ParameterListBuilder : CoreList<string, IParameter>, IParameterListBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterListBuilder(IEngine engine) : base()
    {
        Engine = engine.ThrowWhenNull();

        ValidateItem = (item) => item.ThrowWhenNull();
        GetKey = (x) => x.ThrowWhenNull().Name;
        ValidateKey = (key) => key.NotNullNotEmpty();
        CompareKeys = (x, y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
        ExpandItems = false;
        GetDuplicates = IndexesOf;
        CanInclude = (item, x) => ReferenceEquals(item, x)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterListBuilder(IEngine engine, IParameter item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterListBuilder(
        IEngine engine, IEnumerable<IParameter> range) : this(engine) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterListBuilder(ParameterListBuilder source) : this(source.Engine) => AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => base.ToString();

    /// <inheritdoc/>
    public virtual ParameterList ToInstance()
    {
        return
            Count == 0 ? new(Engine) :
            Count == 1 ? new(Engine, this[0]) : new(Engine, this);
    }
    IParameterList IParameterListBuilder.ToInstance() => ToInstance();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string NextName()
    {
        for (int i = 0; i < int.MaxValue; i++)
        {
            var name = $"{Engine.ParametersPrefix}{i}";
            var index = IndexOf(name);
            if (index < 0) return name;
        }
        throw new UnExpectedException("Range of integers exhausted.");
    }

    /// <inheritdoc/>
    public virtual int AddNew(object? value, out IParameter item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }

    /// <inheritdoc/>
    public virtual int InsertNew(int index, object? value, out IParameter item)
    {
        item = new Parameter(NextName(), value);
        return Insert(index, item);
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    protected override bool SameItem(IParameter source, IParameter item)
    {
        return ReferenceEquals(source, item);
    }
}