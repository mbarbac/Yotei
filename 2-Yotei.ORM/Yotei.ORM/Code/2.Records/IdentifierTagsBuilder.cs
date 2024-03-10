using T = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierTagsBuilder : CoreList<T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public IdentifierTagsBuilder(bool caseSensitiveTags)
    {
        CaseSensitiveTags = caseSensitiveTags;

        Validate = (item) =>
        {
            item.ThrowWhenNull();

            if (CaseSensitiveTags != item.CaseSensitiveTags) throw new ArgumentException(
                "Element's Case Sensitive setting is not the same of this collection.")
                .WithData(item)
                .WithData(this);

            return item;
        };
        Compare = ReferenceEquals;
        GetDuplicates = (item) => IndexesOf(x => x.ContainsAny(item));
        CanInclude = (item, _) => throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="item"></param>
    public IdentifierTagsBuilder(
        bool caseSensitiveTags, T item) : this(caseSensitiveTags) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTagsBuilder(
        bool caseSensitiveTags, IEnumerable<T> range) : this(caseSensitiveTags) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTagsBuilder(
        IdentifierTagsBuilder source) : this(source.CaseSensitiveTags) => AddRange(source);

    /// <summary>
    /// Returns a new instance based upon the contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IIdentifierTags ToInstance() => new IdentifierTags(CaseSensitiveTags, this);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the names in this instance are considered case sensitive or not.
    /// </summary>
    public bool CaseSensitiveTags { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <summary>
    /// Determines if this collection contains any name from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> range) => IndexOfAny(range) >= 0;

    /// <summary>
    /// Gets the index of the element in this collection that carries the given name, or -1 if it
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
    /// Gets the index of the first element in this instance that carries any of the given names
    /// or -1 if such element is not found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int IndexOfAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return -1;
        if (range is ICollection irange && irange.Count == 0) return -1;

        return IndexOf(x =>
        {
            foreach (var name in range) if (x.Contains(name)) return true;
            return false;
        });
    }

    /// <summary>
    /// Gets the index of the last element in this instance that carries any of the given names
    /// or -1 if such element is not found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int LastIndexOfAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return -1;
        if (range is ICollection irange && irange.Count == 0) return -1;

        return LastIndexOf(x =>
        {
            foreach (var name in range) if (x.Contains(name)) return true;
            return false;
        });
    }

    /// <summary>
    /// Gets the indexes of the elements in this instance that carries any of the given names.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<int> IndexesOfAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return [];
        if (range is ICollection irange && irange.Count == 0) return [];

        return IndexesOf(x =>
        {
            foreach (var name in range) if (x.Contains(name)) return true;
            return false;
        });
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this collection the element that carries the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int Remove(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the first element that carries any of the names from the
    /// given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int RemoveAny(IEnumerable<string> range)
    {
        var index = IndexOfAny(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last element that carries any of the names from the
    /// given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int RemoveLastAny(IEnumerable<string> range)
    {
        var index = LastIndexOfAny(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements that carry any of the names from the given
    /// range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int RemoveAllAny(IEnumerable<string> range)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOfAny(range);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }
}