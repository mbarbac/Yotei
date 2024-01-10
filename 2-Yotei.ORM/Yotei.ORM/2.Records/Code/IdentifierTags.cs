namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that contains the ordered collection of metadata tags that describes the
/// maximal structure of the identifiers in an underlying database. Each carries a collection of
/// valid names for that metadata entry, and an optional default value.
/// <br/> Duplicated tag names are not allowed.
/// </summary>
[Cloneable]
public sealed partial class IdentifierTags : IEnumerable<MetadataTag>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public IdentifierTags(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="tag"></param>
    public IdentifierTags(IEngine engine, MetadataTag tag) : this(engine) => AddInternal(tag);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierTags(IEngine engine, IEnumerable<MetadataTag> range)
        : this(engine)
        => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(IdentifierTags source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<MetadataTag> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join('.', Items.Select(x => x[0]));

    // ----------------------------------------------------

    readonly List<MetadataTag> Items = [];

    MetadataTag Validate(MetadataTag tag)
    {
        tag.ThrowWhenNull();

        if (Engine != tag.Engine) throw new ArgumentException(
            "Engine of the given tag is not the engine of this instance.")
            .WithData(tag);

        foreach (var name in tag)
            if (name.Contains('.')) throw new ArgumentException(
                "Identifier names cannot contain dots.")
                .WithData(tag);

        return tag;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Gets the collection of names carried by this instance.
    /// </summary>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var item in Items)
                foreach (var name in item) yield return name;
        }
    }

    /// <summary>
    /// Gets the number of tags in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the tag stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MetadataTag this[int index] => Items[index];

    /// <summary>
    /// Determines if this collection contains a tag with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <summary>
    /// Determines if this collection contains a tag with any name from the given range.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> names) => IndexOfAny(names) >= 0;

    /// <summary>
    /// Returns the index of the tag in this collection that contains the given name, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int IndexOf(string name)
    {
        name = name.NotNullNotEmpty();
        return IndexOf(x => x.Contains(name));
    }

    /// <summary>
    /// Returns the index of the tag in this collection that contains any of the given names, or
    /// -1 if any can be found.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public int IndexOfAny(IEnumerable<string> names)
    {
        names.ThrowWhenNull();
        return IndexOf(x => x.ContainsAny(names));
    }

    /// <summary>
    /// Determines if this collection contains a tag that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<MetadataTag> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of a tag that matches the given predicate, or -1
    /// if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<MetadataTag> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of a tag that matches the given predicate, or -1
    /// if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<MetadataTag> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the tags in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<MetadataTag> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// Returns an array with the tags in this collection.
    /// </summary>
    /// <returns></returns>
    public MetadataTag[] ToArray() => Items.ToArray();

    /// <summary>
    /// Returns a list with the tags in this collection.
    /// </summary>
    /// <returns></returns>
    public List<MetadataTag> ToList() => new(Items);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of tags starting from the given index. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IdentifierTags GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the tag at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public IdentifierTags Replace(int index, MetadataTag tag)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, tag);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, MetadataTag tag)
    {
        tag = Validate(tag);

        if (tag == Items[index]) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, tag);
    }

    /// <summary>
    /// Returns a new instance where the given tag has been added to the collection.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public IdentifierTags Add(MetadataTag tag)
    {
        var clone = Clone();
        var num = clone.AddInternal(tag);
        return num > 0 ? clone : this;
    }
    int AddInternal(MetadataTag tag)
    {
        tag = Validate(tag);

        foreach (var item in Items)
            if (item.ContainsAny(tag)) throw new DuplicateException(
                "A name in the given tag is already used in this collection.")
                .WithData(tag);

        Items.Add(tag);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the tags from the given range have been added to it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IdentifierTags AddRange(IEnumerable<MetadataTag> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<MetadataTag> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the given tag has been inserted into the collection at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public IdentifierTags Insert(int index, MetadataTag tag)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, tag);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, MetadataTag tag)
    {
        tag = Validate(tag);

        foreach (var item in Items)
            if (item.ContainsAny(tag)) throw new DuplicateException(
                "A name in the given tag is already used in this collection.")
                .WithData(tag);

        Items.Insert(index, tag);
        return 1;
    }

    /// <summary>
    /// Returns a new instance the tags from the given range have been inserted into it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IdentifierTags InsertRange(int index, IEnumerable<MetadataTag> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<MetadataTag> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the tag at the given index has been removed from the original
    /// collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IdentifierTags RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the given number of tags, starting from the given index,
    /// have been removed from the collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IdentifierTags RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the tag that carries the given name has been removed.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IdentifierTags Remove(string name)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(name);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the tag that carries any of the given names has been removed.
    /// </summary>
    /// <param name="name"></param>
    public IdentifierTags RemoveIfAny(IEnumerable<string> names)
    {
        var clone = Clone();
        var num = clone.RemoveIfAnyInternal(names);
        return num > 0 ? clone : this;
    }
    int RemoveIfAnyInternal(IEnumerable<string> names)
    {
        var index = IndexOfAny(names);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the first tag that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierTags Remove(Predicate<MetadataTag> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<MetadataTag> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the last tag that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierTags RemoveLast(Predicate<MetadataTag> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<MetadataTag> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where all tags that match the given predicate has been removed. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierTags RemoveAll(Predicate<MetadataTag> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<MetadataTag> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where all the tags have been removed. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <returns></returns>
    public IdentifierTags Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}