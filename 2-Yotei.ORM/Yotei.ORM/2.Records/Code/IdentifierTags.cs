using T = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierTags"/>
[Cloneable] 
public sealed partial class IdentifierTags : FrozenList<T>, IIdentifierTags
{
    protected override IdentifierTagsBuilder Items => _Items ??= new(Engine);
    IdentifierTagsBuilder? _Items = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierTags(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierTags(IEngine engine, T item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(
        IdentifierTags source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public int IndexOf(string name) => Items.IndexOf(name);

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range) => Items.ContainsAny(range);

    /// <inheritdoc/>
    public int IndexOfAny(IEnumerable<string> range) => Items.IndexOfAny(range);

    /// <inheritdoc/>
    public int LastIndexOfAny(IEnumerable<string> range) => Items.LastIndexOfAny(range);

    /// <inheritdoc/>
    public List<int> IndexesOfAny(IEnumerable<string> range) => Items.IndexesOfAny(range);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifierTags Remove(string name)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(name);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierTags RemoveAny(IEnumerable<string> range)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAny(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierTags RemoveLastAny(IEnumerable<string> range)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLastAny(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public IIdentifierTags RemoveAllAny(IEnumerable<string> range)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAllAny(range);
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IIdentifierTags GetRange(int index, int count) => (IIdentifierTags)base.GetRange(index, count);

    /// <inheritdoc/>
    public override IIdentifierTags Replace(int index, T item) => (IIdentifierTags)base.Replace(index, item);

    /// <inheritdoc/>
    public override IIdentifierTags Add(T item) => (IIdentifierTags)base.Add(item);

    /// <inheritdoc/>
    public override IIdentifierTags AddRange(IEnumerable<T> range) => (IIdentifierTags)base.AddRange(range);

    /// <inheritdoc/>
    public override IIdentifierTags Insert(int index, T item) => (IIdentifierTags)base.Insert(index, item);

    /// <inheritdoc/>
    public override IIdentifierTags InsertRange(int index, IEnumerable<T> range) => (IIdentifierTags)base.InsertRange(index, range);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveAt(int index) => (IIdentifierTags)base.RemoveAt(index);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveRange(int index, int count) => (IIdentifierTags)base.RemoveRange(index, count);

    /// <inheritdoc/>
    public override IIdentifierTags Remove(T item) => (IIdentifierTags)base.Remove(item);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveLast(T item) => (IIdentifierTags)base.RemoveLast(item);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveAll(T item) => (IIdentifierTags)base.RemoveAll(item);

    /// <inheritdoc/>
    public override IIdentifierTags Remove(Predicate<T> predicate) => (IIdentifierTags)base.Remove(predicate);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveLast(Predicate<T> predicate) => (IIdentifierTags)base.RemoveLast(predicate);

    /// <inheritdoc/>
    public override IIdentifierTags RemoveAll(Predicate<T> predicate) => (IIdentifierTags)base.RemoveAll(predicate);

    /// <inheritdoc/>
    public override IIdentifierTags Clear() => (IIdentifierTags)base.Clear();
}