namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterList"/>
[FrozenList<string, IParameter>]
public sealed partial class ParameterList : IParameterList
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public Builder ToBuilder() => Items.Clone();

    protected override Builder Items { get; }

    // -----------------------------------------------------

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
    public ParameterList AddNew(object? value, out IParameter item)
    {
        var builder = Items.Clone();
        var done = builder.AddNew(value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IParameterList IParameterList.AddNew(object? value, out IParameter item) => AddNew(value, out item);

    /// <inheritdoc/>
    public ParameterList InsertNew(int index, object? value, out IParameter item)
    {
        var builder = Items.Clone();
        var done = builder.InsertNew(index, value, out item);
        return done > 0 ? builder.ToInstance() : this;
    }
    IParameterList IParameterList.InsertNew(int index, object? value, out IParameter item) => InsertNew(index, value, out item);

    // ====================================================

    /// <summary>
    /// Represents a builder of <see cref="IParameterList"/> instances.
    /// </summary>
    [Cloneable]
    public sealed partial class Builder : CoreList<string, IParameter>
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base()
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
        public Builder(IEngine engine, IParameter item) : this(engine) => Add(item);

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IParameter> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        Builder(Builder source) : this(source.Engine) => AddRange(source);

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the captured contents.
        /// </summary>
        /// <returns></returns>
        public ParameterList ToInstance() => new(Engine, this);

        /// <inheritdoc cref="IParameterList.Engine"/>
        public IEngine Engine { get; }

        /// <inheritdoc cref="IParameterList.NextName"/>
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

        /// <inheritdoc cref="IParameterList.AddNew(object?, out IParameter)"/>
        public int AddNew(object? value, out IParameter item)
        {
            item = new Parameter(NextName(), value);
            return Add(item);
        }

        /// <inheritdoc cref="IParameterList.InsertNew(int, object?, out IParameter)"/>
        public int InsertNew(int index, object? value, out IParameter item)
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
}