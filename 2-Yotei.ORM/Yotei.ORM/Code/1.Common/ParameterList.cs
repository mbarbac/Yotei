using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterList"/>
[Cloneable]
public sealed partial class ParameterList : FrozenList<K?, T>, IParameterList
{
    protected override ParameterListBuilder Items => _Items ??= new(Engine);
    ParameterListBuilder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public ParameterList(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source);

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public ParameterListBuilder ToBuilder() => new(Engine, this);

    /// <inheritdoc/>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IParameterList GetRange(int index, int count) => (IParameterList)base.GetRange(index, count);

    /// <inheritdoc/>
    public override IParameterList Replace(int index, T item) => (IParameterList)base.Replace(index, item);

    /// <inheritdoc/>
    public override IParameterList Add(T item) => (IParameterList)base.Add(item);

    /// <inheritdoc/>
    public override IParameterList AddRange(IEnumerable<T> range) => (IParameterList)base.AddRange(range);

    /// <inheritdoc/>
    public override IParameterList Insert(int index, T item) => (IParameterList)base.Insert(index, item);

    /// <inheritdoc/>
    public override IParameterList InsertRange(int index, IEnumerable<T> range) => (IParameterList)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override IParameterList RemoveAt(int index) => (IParameterList)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IParameterList RemoveRange(int index, int count) => (IParameterList)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IParameterList Remove(K? key) => (IParameterList)base.Remove(key);

    /// <inheritdoc/>
    public override IParameterList RemoveLast(K? key) => (IParameterList)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IParameterList RemoveAll(K? key) => (IParameterList)base.RemoveAll(key);

    /// <inheritdoc/>
    public override IParameterList Remove(Predicate<T> predicate) => (IParameterList)base.Remove(predicate);

    /// <inheritdoc/>
    public override IParameterList RemoveLast(Predicate<T> predicate) => (IParameterList)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IParameterList RemoveAll(Predicate<T> predicate) => (IParameterList)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IParameterList Clear() => (IParameterList)base.Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

    /// <inheritdoc/>
    public IParameterList AddNew(object? value, out T item)
    {
        var clone = Clone();
        var num = clone.Items.AddNew(value, out item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IParameterList InsertNew(int index, object? value, out T item)
    {
        var clone = Clone();
        var num = clone.Items.InsertNew(index, value, out item);
        return num > 0 ? clone : this;
    }
}