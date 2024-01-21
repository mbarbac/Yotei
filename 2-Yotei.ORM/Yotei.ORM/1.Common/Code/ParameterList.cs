using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameterList"/>
/// </summary>
[Cloneable]
public sealed partial class ParameterList : FrozenList<TKey, TItem>, IParameterList
{
    protected override ParameterListBuilder Items => _Items ??= new ParameterListBuilder(Engine);
    ParameterListBuilder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, TItem item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string NextName() => Items.NextName();

    // ----------------------------------------------------

    public override IParameterList GetRange(int index, int count) => (IParameterList)base.GetRange(index, count);
    public override IParameterList Replace(int index, TItem item) => (IParameterList)base.Replace(index, item);
    public override IParameterList Add(TItem item) => (IParameterList)base.Add(item);
    public override IParameterList AddRange(IEnumerable<TItem> range) => (IParameterList)base.AddRange(range);
    public override IParameterList Insert(int index, TItem item) => (IParameterList)base.Insert(index, item);
    public override IParameterList InsertRange(int index, IEnumerable<TItem> range) => (IParameterList)base.InsertRange(index, range);
    public override IParameterList RemoveAt(int index) => (IParameterList)base.RemoveAt(index);
    public override IParameterList RemoveRange(int index, int count) => (IParameterList)base.RemoveRange(index, count);
    public override IParameterList Remove(TKey key) => (IParameterList)base.Remove(key);
    public override IParameterList RemoveLast(TKey key) => (IParameterList)base.RemoveLast(key);
    public override IParameterList RemoveAll(TKey key) => (IParameterList)base.RemoveAll(key);
    public override IParameterList Remove(Predicate<TItem> predicate) => (IParameterList)base.Remove(predicate);
    public override IParameterList RemoveLast(Predicate<TItem> predicate) => (IParameterList)base.RemoveLast(predicate);
    public override IParameterList RemoveAll(Predicate<TItem> predicate) => (IParameterList)base.RemoveAll(predicate);
    public override IParameterList Clear() => (IParameterList)base.Clear();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList AddNew(object? value, out TItem? item)
    {
        var clone = Clone();
        var num = clone.Items.AddNew(value, out item);
        return num > 0 ? clone : this;
    }
}