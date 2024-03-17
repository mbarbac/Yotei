#pragma warning disable IDE0290 // Use primary constructor

using T = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierTags"/>
[Cloneable]
public sealed partial class IdentifierTags : FrozenList<T>, IIdentifierTags
{
    protected override IdentifierTagsBuilder Items => _Items ??= new(CaseSensitiveTags);
    IdentifierTagsBuilder? _Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitive"></param>
    public IdentifierTags(bool caseSensitive) => CaseSensitiveTags = caseSensitive;

    /// <summary>
    /// Initializes a new instance with the given element
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="item"></param>
    public IdentifierTags(bool caseSensitive, T item) : this(caseSensitive) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool caseSensitive, IEnumerable<T> range) : this(caseSensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(
        IdentifierTags source) : this(source.CaseSensitiveTags) => Items.AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifierTags? other)
    {
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++) if (!this[i].Equals(other[i])) return false;
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IIdentifierTags);
    public static bool operator ==(IdentifierTags x, IIdentifierTags y) => x is not null && x.Equals(y);
    public static bool operator !=(IdentifierTags x, IIdentifierTags y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(CaseSensitiveTags);
        for (int i = 0; i < Items.Count; i++) code = HashCode.Combine(code, Items[i]);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Names => Items.Names;

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