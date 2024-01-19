using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Represents a builder of <see cref="IParameterList"/> instances.
/// </summary>
[Cloneable]
public partial class ParameterListBuilder : CoreList<TKey, TItem>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterListBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterListBuilder(IEngine engine, TItem item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterListBuilder(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ParameterListBuilder(ParameterListBuilder source)
        : this(source.Engine)
        => AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override TItem ValidateItem(TItem item) => item.ThrowWhenNull();

    /// <inheritdoc/>
    protected override TKey GetKey(TItem item) => item.ThrowWhenNull().Name;

    /// <inheritdoc/>
    protected override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();

    /// <inheritdoc/>
    protected override bool CompareKeys(TKey source, TKey item)
        => string.Compare(source, item, !Engine.CaseSensitiveNames) == 0;

    /// <inheritdoc/>
    protected override bool SameItem(TItem source, TItem item)
        => ReferenceEquals(source, item);

    /// <inheritdoc/>
    protected override List<int> GetDuplicates(TKey key) => base.GetDuplicates(key);

    /// <inheritdoc/>
    protected override bool AcceptDuplicate(TItem source, TItem item)
        => ReferenceEquals(source, item)
        ? true
        : throw new DuplicateException("Duplicated element.").WithData(item);

    // ----------------------------------------------------

    /// <summary>
    /// The identifier this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Returns the next available parameter name.
    /// </summary>
    /// <returns></returns>
    public string NextName()
    {
        for (int i = Count; i < int.MaxValue; i++)
        {
            var name = $"{Engine.ParameterPrefix}{i}";
            var index = IndexOf(name);
            if (index < 0) return name;
        }
        throw new UnExpectedException("Range of integers exhausted.");
    }

    /// <summary>
    /// Adds to this instance a new element built using the given value and the next available
    /// name. Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int AddNew(object? value, out TItem? item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }
}