namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents a region.
/// </summary>
public class Region
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Region()
    {
        _Parent = new(this, r => r.Regions);
        _Regions = new(this, r => r.Parent, (r, v) => r.Parent = v, r => r.Regions);
        _Countries = new(this, c => c.Region, (c, v) => c.Region = v, r => r.Countries);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Region(string id, string name) : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        sb.Append($", Name:{Name ?? "-"}");
        if (Parent != null) sb.Append($", Parent:{Parent.Id ?? "-"}");
        if (Regions.Count != 0) sb.Append($", Regions:[{GetRegions()}]");
        if (Countries.Count != 0) sb.Append($", Countries:[{GetCountries()}]");
        return sb.ToString();

        string GetRegions() => string.Join(", ", _Regions.Select(x => x.Id ?? "-"));
        string GetCountries() => string.Join(", ", _Countries.Select(x => x.Id ?? "-"));
    }

    /// <summary>
    /// The id of this instance, or null.
    /// </summary>
    public string? Id
    {
        get => _Id;
        set => _Id = value?.NotNullNotEmpty();
    }
    string? _Id = null;

    /// <summary>
    /// The name of this instance, or null.
    /// </summary>
    public string? Name
    {
        get => _Name;
        set => _Name = value?.NotNullNotEmpty();
    }
    string? _Name = null;

    /// <summary>
    /// The parent region of this one, or null.
    /// </summary>
    public Region? Parent
    {
        get => _Parent.Value;
        set => _Parent.Value = value;
    }
    XParent<Region, Region> _Parent;

    /// <summary>
    /// The collection of child regions in this one.
    /// </summary>
    public ICollection<Region> Regions => _Regions;
    XChilds<Region, Region> _Regions;

    /// <summary>
    /// The collection of countries in this region.
    /// </summary>
    public ICollection<Country> Countries => _Countries;
    XChilds<Region, Country> _Countries;
}