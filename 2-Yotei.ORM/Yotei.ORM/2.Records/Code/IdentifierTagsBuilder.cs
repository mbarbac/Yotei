using T = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierTagsBuilder : CoreList<T>
{
    public IdentifierTagsBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Validate = (item) =>
        {
            item.ThrowWhenNull();
            if (!Engine.Equals(item.Engine)) throw new ArgumentException(
                "Engine of given element is not the engine of this collection.")
                .WithData(item)
                .WithData(this);

            return item;
        };
        Compare = ReferenceEquals;
        Duplicates = (@this, item) => @this.IndexesOf(x => x.ContainsAny(item));
        CanInclude = (_, item) => throw new DuplicateException("Duplicated element.").WithData(item);
    }
    public IdentifierTagsBuilder(IEngine engine, T item) : this(engine) => Add(item);
    public IdentifierTagsBuilder(
        IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);
    IdentifierTagsBuilder(
        IdentifierTagsBuilder source) : this(source.Engine) => AddRange(source);

    public IEngine Engine { get; }

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