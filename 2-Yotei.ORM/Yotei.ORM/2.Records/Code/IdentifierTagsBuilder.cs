using T = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierTagsBuilder : CoreList<T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitive"></param>
    public IdentifierTagsBuilder(bool caseSensitive)
    {
        CaseSensitiveTags = caseSensitive;

        ValidateItem = (item) =>
        {
            item.ThrowWhenNull();

            if (CaseSensitiveTags != item.CaseSensitiveTags) throw new ArgumentException(
                "CaseSensitive setting of the given element is not the same of this collection.")
                .WithData(item)
                .WithData(this);

            return item;
        };
        CompareItems = (x, y) => ReferenceEquals(x, y) || x.Equals(y);
        GetDuplicates = (item) => IndexesOf(x => x.Contains(item));
        CanInclude = (item, _) => throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Initializes a new instance with the given element
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="item"></param>
    public IdentifierTagsBuilder(bool caseSensitive, T item) : this(caseSensitive) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="range"></param>
    public IdentifierTagsBuilder(
        bool caseSensitive, IEnumerable<T> range) : this(caseSensitive) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTagsBuilder(
        IdentifierTagsBuilder source) : this(source.CaseSensitiveTags) => AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc cref="IIdentifierTags.CaseSensitiveTags"/>
    public bool CaseSensitiveTags { get; }

    /// <inheritdoc cref = "IIdentifierTags.Names"/>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var item in this)
                foreach (var name in item) yield return name;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IIdentifierTags.Contains(string)"/>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <inheritdoc cref="IIdentifierTags.Contains(IEnumerable{string})"/>
    public bool Contains(IEnumerable<string> range) => IndexOf(range) >= 0;

    /// <inheritdoc cref="IIdentifierTags.IndexOf(string)"/>
    public int IndexOf(string name)
    {
        name = name.NotNullNotEmpty();
        return IndexOf(x => x.Contains(name));
    }

    /// <<inheritdoc cref="IIdentifierTags.IndexOf(IEnumerable{string})"/>
    public int IndexOf(IEnumerable<string> range)
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

    /// <inheritdoc cref="IIdentifierTags.LastIndexOf(IEnumerable{string})"/>
    public int LastIndexOf(IEnumerable<string> range)
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

    /// <inheritdoc cref="IIdentifierTags.IndexesOf(IEnumerable{string})"/>
    public List<int> IndexesOf(IEnumerable<string> range)
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
    /// Removes the element that carries the given tag name. Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes the first element that carries any tag name from the given range. Returns the
    /// number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int Remove(IEnumerable<string> range)
    {
        var index = IndexOf(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes the last element that carries any tag name from the given range. Returns the
    /// number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int RemoveLast(IEnumerable<string> range)
    {
        var index = LastIndexOf(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes all the elements that carry any tag name from the given range. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int RemoveAll(IEnumerable<string> range)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(range);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }
}