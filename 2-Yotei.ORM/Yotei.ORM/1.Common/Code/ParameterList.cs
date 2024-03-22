using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IParameterList"/>
[Cloneable]
public sealed partial class ParameterList : FrozenList<K, T>, IParameterList
{
    protected override ParameterListBuilder Items { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public ParameterList(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterList(ParameterList source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IParameterList? other)
    {
        if (other is null) return false;
        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++) if (!Items[i].EquivalentTo(other[i])) return false;
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IParameterList);
    public static bool operator ==(ParameterList x, IParameterList y) => x is not null && x.Equals(y);
    public static bool operator !=(ParameterList x, IParameterList y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string NextName() => Items.NextName();

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
    public override IParameterList Remove(K key) => (IParameterList)base.Remove(key);

    /// <inheritdoc/>
    public override IParameterList RemoveLast(K key) => (IParameterList)base.RemoveLast(key);

    /// <inheritdoc/>
    public override IParameterList RemoveAll(K key) => (IParameterList)base.RemoveAll(key);

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
    public IParameterList AddNew(object? value, out T item)
    {
        var clone = Clone(); clone.Items.AddNew(value, out item);
        return clone;
    }

    /// <inheritdoc/>
    public IParameterList InsertNew(int index, object? value, out T item)
    {
        var clone = Clone(); clone.Items.InsertNew(index, value, out item);
        return clone;
    }
}