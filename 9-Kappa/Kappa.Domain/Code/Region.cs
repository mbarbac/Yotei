namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents a region.
/// </summary>
public class Region
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    public Region(string id)
    {
        Id = id;
        Regions = new(this);
        Countries = new(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Id);
        if (Name != null) sb.Append($", Name:{Name}");
        if (Parent != null) sb.Append($", Parent:{Parent.Id}");
        if (Regions.Count != 0) sb.Append($", Childs:{Regions.Select(x => x.Id).Sketch()}");
        if (Countries.Count != 0) sb.Append($", Countries:{Countries.Select(x => x.Id).Sketch()}");
        return sb.ToString();
    }

    /// <summary>
    /// The unique id of this region.
    /// </summary>
    public string Id
    {
        get => _Id;
        init => _Id = value.NotNullNotEmpty();
    }
    string _Id = default!;

    /// <summary>
    /// The name of this region, or null.
    /// </summary>
    public string? Name
    {
        get => _Name;
        init => _Name = value?.NotNullNotEmpty();
    }
    string? _Name = default!;

    /// <summary>
    /// The parent region this instance belongs to, or null.
    /// </summary>
    public Region? Parent
    {
        get => _Parent;
        set
        {
            if (_Parent == value) return;

            _Parent?.Regions.Remove(this);
            _Parent = value;
            _Parent?.Regions.Add(this);
        }
    }
    Region? _Parent;

    /// <summary>
    /// The list of child regions that belong to this one.
    /// </summary>
    public RegionList Regions { get; }
    public class RegionList : CustomList<Region>
    {
        readonly Region Master;
        internal RegionList(Region master)
        {
            Master = master;
            Validate = (item, add) => item.ThrowWhenNull();
            Compare = (source, target) => source.Id == target.Id;
            AcceptDuplicate = (source, target)
                => ReferenceEquals(source, target)
                ? false
                : throw new ArgumentException("Duplicate item.").WithData(target);
        }
        protected override string Item2String(Region item) => item.Id;
        public override int Add(Region item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Add(item);
            if (r > 0) item.Parent = Master;
            return r;
        }
        public override int Insert(int index, Region item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Insert(index, item);
            if (r > 0) item.Parent = Master;
            return r;
        }
        public override int RemoveAt(int index)
        {
            var item = this[index];
            var r = base.RemoveAt(index);
            if (r > 0) item.Parent = null;
            return r;
        }
    }

    /// <summary>
    /// The countries that belong to this region.
    /// </summary>
    public CountryList Countries { get; }
    public class CountryList : CustomList<Country>
    {
        readonly Region Master;
        internal CountryList(Region master)
        {
            Master = master;
            Validate = (item, add) => item.ThrowWhenNull();
            Compare = (source, target) => source.Id == target.Id;
            AcceptDuplicate = (source, target)
                => ReferenceEquals(source, target)
                ? false
                : throw new ArgumentException("Duplicate item.").WithData(target);
        }
        protected override string Item2String(Country item) => item.Id;
        public override int Add(Country item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Add(item);
            if (r > 0) item.Region = Master;
            return r;
        }
        public override int Insert(int index, Country item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Insert(index, item);
            if (r > 0) item.Region = Master;
            return r;
        }
        public override int RemoveAt(int index)
        {
            var item = this[index];
            var r = base.RemoveAt(index);
            if (r > 0) item.Region = null;
            return r;
        }
    }
}