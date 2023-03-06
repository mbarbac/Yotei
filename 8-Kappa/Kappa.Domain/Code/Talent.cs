namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents a talent.
/// </summary>
public class Talent
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Talent()
    {
        _Employees = new(this, e => e.Talents);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Talent(string id, string name) : this()
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
    /// The collection of employees that have this talent.
    /// </summary>
    public ICollection<Employee> Employees => _Employees;
    XTangled<Talent, Employee> _Employees;
}