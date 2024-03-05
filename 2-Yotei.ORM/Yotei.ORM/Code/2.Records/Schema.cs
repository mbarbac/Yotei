using T = Yotei.ORM.ISchemaEntry;
using K = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ISchema"/>
[Cloneable]
public sealed partial class Schema : FrozenList<K, T>, ISchema
{
    protected override SchemaBuilder Items => _Items ??= new(Engine);
    SchemaBuilder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public Schema(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Schema(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Schema(Schema source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string identifier) => Items.Contains(identifier);

    /// <inheritdoc/>
    public int IndexOf(string identifier) => Items.IndexOf(identifier);

    /// <inheritdoc/>
    public int LastIndexOf(string identifier) => Items.LastIndexOf(identifier);

    /// <inheritdoc/>
    public List<int> IndexesOf(string identifier) => Items.IndexesOf(identifier);

    /// <inheritdoc/>
    public List<int> Match(string? specs) => Items.Match(specs);

    /// <inheritdoc/>
    public List<int> Match(string? specs, out T? unique) => Items.Match(specs, out unique);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override ISchema GetRange(int index, int count) => (ISchema)base.GetRange(index, count);

    /// <inheritdoc/>
    public override ISchema Replace(int index, T item) => (ISchema)base.Replace(index, item);

    /// <inheritdoc/>
    public override ISchema Add(T item) => (ISchema)base.Add(item);

    /// <inheritdoc/>
    public override ISchema AddRange(IEnumerable<T> range) => (ISchema)base.AddRange(range);

    /// <inheritdoc/>
    public override ISchema Insert(int index, T item) => (ISchema)base.Insert(index, item);

    /// <inheritdoc/>
    public override ISchema InsertRange(int index, IEnumerable<T> range) => (ISchema)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override ISchema RemoveAt(int index) => (ISchema)base.RemoveAt(index);

    /// <inheritdoc/>
    public override ISchema RemoveRange(int index, int count) => (ISchema)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override ISchema Remove(K key) => (ISchema)base.Remove(key);

    /// <inheritdoc/>
    public override ISchema RemoveLast(K key) => (ISchema)base.RemoveLast(key);

    /// <inheritdoc/>
    public override ISchema RemoveAll(K key) => (ISchema)base.RemoveAll(key);

    /// <inheritdoc/>
    public override ISchema Remove(Predicate<T> predicate) => (ISchema)base.Remove(predicate);

    /// <inheritdoc/>
    public override ISchema RemoveLast(Predicate<T> predicate) => (ISchema)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override ISchema RemoveAll(Predicate<T> predicate) => (ISchema)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override ISchema Clear() => (ISchema)base.Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema Remove(string identifier)
    {
        var clone = Clone();
        var done = clone.Items.Remove(identifier);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchema RemoveLast(string identifier)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(identifier);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchema RemoveAll(string identifier)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(identifier);
        return done > 0 ? clone : this;
    }
}