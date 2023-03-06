namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents a country.
/// </summary>
public class Country
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Country()
    {
        _Region = new(this, r => r.Countries);
        _Employees = new(this, e => e.Country, (e, v) => e.Country = v, c => c.Employees);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Country(string id, string name) : this()
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
        if (Region != null) sb.Append($", Region:{Region.Id ?? "-"}");
        if (Employees.Count != 0) sb.Append($", Employees:[{GetEmployees()}]");
        return sb.ToString();

        string GetEmployees() => string.Join(", ", _Employees.Select(x => x.Id ?? "-"));
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
    /// The parent region of this country, or null.
    /// </summary>
    public Region? Region
    {
        get => _Region.Value;
        set => _Region.Value = value;
    }
    XParent<Country, Region> _Region;

    /// <summary>
    /// The collection of employees in this country.
    /// </summary>
    public ICollection<Employee> Employees => _Employees;
    XChilds<Country, Employee> _Employees;
}