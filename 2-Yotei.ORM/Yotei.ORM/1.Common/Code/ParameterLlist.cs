using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameterList"/>
/// </summary>
[Cloneable]
public sealed partial class ParameterList : FrozenList<K, T>, IParameterList
{
    public IEngine Engine { get; }

    protected override ParameterListBuilder Items => _Items ??= new(Engine);
    ParameterListBuilder? _Items = null;

    public ParameterList(IEngine engine) => Engine = engine.ThrowWhenNull();
    public ParameterList(IEngine engine, T item) : this(engine) => Items.Add(item);
    public ParameterList(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);
    ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source);

    public override ParameterListBuilder ToBuilder() => (ParameterListBuilder)base.ToBuilder();
    public override IParameterList GetRange(int index, int count) => (IParameterList)base.GetRange(index, count);
    public override IParameterList Replace(int index, T item) => (IParameterList)base.Replace(index, item);
    public override IParameterList Add(T item) => (IParameterList)base.Add(item);
    public override IParameterList AddRange(IEnumerable<T> range) => (IParameterList)base.AddRange(range);
    public override IParameterList Insert(int index, T item) => (IParameterList)base.Insert(index, item);
    public override IParameterList InsertRange(int index, IEnumerable<T> range) => (IParameterList)base.InsertRange(index, range);
    public override IParameterList RemoveAt(int index) => (IParameterList)base.RemoveAt(index);
    public override IParameterList RemoveRange(int index, int count) => (IParameterList)base.RemoveRange(index, count);
    public override IParameterList Remove(K key) => (IParameterList)base.Remove(key);
    public override IParameterList RemoveLast(K key) => (IParameterList)base.RemoveLast(key);
    public override IParameterList RemoveAll(K key) => (IParameterList)base.RemoveAll(key);
    public override IParameterList Remove(Predicate<T> predicate) => (IParameterList)base.Remove(predicate);
    public override IParameterList RemoveLast(Predicate<T> predicate) => (IParameterList)base.RemoveLast(predicate);
    public override IParameterList RemoveAll(Predicate<T> predicate) => (IParameterList)base.RemoveAll(predicate);
    public override IParameterList Clear() => (IParameterList)base.Clear();

    public string NextName() => Items.NextName();
    public IParameterList AddNew(object? value, out T? item)
    {
        var clone = Clone();
        var num = clone.Items.AddNew(value, out item);
        return num > 0 ? clone : this;
    }
    public IParameterList InsertNew(int index, object? value, out T? item)
    {
        var clone = Clone();
        var num = clone.Items.InsertNew(index, value, out item);
        return num > 0 ? clone : this;
    }
}