using T = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierTagsBuilder : CoreList<T>
{
    public IdentifierTagsBuilder(bool caseSensitiveTags)
    {
        CaseSensitiveTags = caseSensitiveTags;
        Validate = (item) =>
        {
            item.ThrowWhenNull();

            if (caseSensitiveTags != item.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given element is not the same of this collection.")
                .WithData(item)
                .WithData(this);

            return item;
        };
        Compare = ReferenceEquals;
        Duplicates = (@this, item) => @this.IndexesOf(x => x.ContainsAny(item));
        CanInclude = (_, item) => throw new DuplicateException("Duplicated element.").WithData(item);
    }
    public IdentifierTagsBuilder(
        bool caseSensitiveTags, T item) : this(caseSensitiveTags) => Add(item);
    public IdentifierTagsBuilder(
        bool caseSensitiveTags, IEnumerable<T> range) : this(caseSensitiveTags) => AddRange(range);
    IdentifierTagsBuilder(
        IdentifierTagsBuilder source) : this(source.CaseSensitiveTags) => AddRange(source);

    bool CaseSensitiveTags { get; }

    // ----------------------------------------------------

    public bool Contains(string name) => IndexOf(name) >= 0;

    public int IndexOf(string name)
    {
        name = name.NotNullNotEmpty();
        return IndexOf(x => x.Contains(name));
    }

    public bool ContainsAny(IEnumerable<string> range) => IndexOfAny(range) >= 0;

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

    public int Remove(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    public int RemoveAny(IEnumerable<string> range)
    {
        var index = IndexOfAny(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    public int RemoveLastAny(IEnumerable<string> range)
    {
        var index = LastIndexOfAny(range);
        return index >= 0 ? RemoveAt(index) : 0;
    }

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