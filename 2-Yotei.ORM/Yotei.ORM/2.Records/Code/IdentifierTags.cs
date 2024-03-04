using T = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierTags"/>
[Cloneable]
public partial class IdentifierTags : FrozenList<T>, IIdentifierTags
{
    protected override IdentifierTagsBuilder Items => _Items ??= new(CaseSensitiveTags);
    IdentifierTagsBuilder? _Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public IdentifierTags(bool caseSensitiveTags) => CaseSensitiveTags = caseSensitiveTags;

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="item"></param>
    public IdentifierTags(
        bool caseSensitiveTags, T item) : this(caseSensitiveTags) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool caseSensitiveTags, IEnumerable<T> range) : this(caseSensitiveTags) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(IdentifierTags source) : this(source.CaseSensitiveTags) => Items.AddRange(source);

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var item in this)
                foreach (var name in item) yield return name;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range) => Items.ContainsAny(range);

    /// <inheritdoc/>
    public int IndexOf(string name) => Items.IndexOf(name);

    /// <inheritdoc/>
    public int IndexOfAny(IEnumerable<string> range) => Items.IndexOfAny(range);

    /// <inheritdoc/>
    public int LastIndexOfAny(IEnumerable<string> range) => Items.LastIndexOfAny(range);

    /// <inheritdoc/>
    public List<int> IndexesOfAny(IEnumerable<string> range) => Items.IndexesOfAny(range);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{T}.GetRange(int, int)"/>
    public override IIdentifierTags GetRange(int index, int count) => (IIdentifierTags)base.GetRange(index, count);

    /// <inheritdoc cref="IFrozenList{T}.Replace(int, T)"/>
    public override IIdentifierTags Replace(int index, T item) => (IIdentifierTags)base.Replace(index, item);

    /// <inheritdoc cref="IFrozenList{T}.Add(T)"/>
    public override IIdentifierTags Add(T item) => (IIdentifierTags)base.Add(item);

    /// <inheritdoc cref="IFrozenList{T}.AddRange(IEnumerable{T})"/>
    public override IIdentifierTags AddRange(IEnumerable<T> range) => (IIdentifierTags)base.AddRange(range);

    /// <inheritdoc cref="IFrozenList{T}.Insert(int, T)"/>
    public override IIdentifierTags Insert(int index, T item) => (IIdentifierTags)base.Insert(index, item);

    /// <inheritdoc cref="IFrozenList{T}.InsertRange(int, IEnumerable{T})"/>
    public override IIdentifierTags InsertRange(int index, IEnumerable<T> range) => (IIdentifierTags)base.InsertRange(index, range);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAt(int)"/>
    public override IIdentifierTags RemoveAt(int index) => (IIdentifierTags)base.RemoveAt(index);

    /// <inheritdoc cref="IFrozenList{T}.RemoveRange(int, int)"/>
    public override IIdentifierTags RemoveRange(int index, int count) => (IIdentifierTags)base.RemoveRange(index, count);

    /// <inheritdoc cref="IFrozenList{T}.Remove(T)"/>
    public override IIdentifierTags Remove(T item) => (IIdentifierTags)base.Remove(item);

    /// <inheritdoc cref="IFrozenList{T}.RemoveLast(T)"/>
    public override IIdentifierTags RemoveLast(T item) => (IIdentifierTags)base.RemoveLast(item);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAll(T)"/>
    public override IIdentifierTags RemoveAll(T item) => (IIdentifierTags)base.RemoveAll(item);

    /// <inheritdoc cref="IFrozenList{T}.Remove(Predicate{T})"/>
    public override IIdentifierTags Remove(Predicate<T> predicate) => (IIdentifierTags)base.Remove(predicate);

    /// <inheritdoc cref="IFrozenList{T}.RemoveLast(Predicate{T})"/>
    public override IIdentifierTags RemoveLast(Predicate<T> predicate) => (IIdentifierTags)base.RemoveLast(predicate);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAll(Predicate{T})"/>
    public override IIdentifierTags RemoveAll(Predicate<T> predicate) => (IIdentifierTags)base.RemoveAll(predicate);

    /// <inheritdoc cref="IFrozenList{T}.Clear"/>
    public override IIdentifierTags Clear() => (IIdentifierTags)base.Clear();

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
}