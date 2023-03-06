namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents an employee
/// </summary>
public class Employee
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Employee()
    {
        _Country = new(this, c => c.Employees);
        _Manager = new(this, m => m.Employees);
        _Employees = new(this, e => e.Manager, (e, v) => e.Manager = v, e => e.Employees);
        _Talents = new(this, t => t.Employees);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Employee(string id, EmployeeName name) : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    public Employee(string id, string firstName, string lastName) : this()
    {
        Id = id;
        Name.FirstName = firstName;
        Name.LastName = lastName;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        sb.Append($", Name:{Name}");
        if (BirthDate != null) sb.Append($", BirthDate:{BirthDate}");
        if (Active != null) sb.Append($", Active:{Active}");
        if (Photo != null) sb.Append($", Photo:[{GetPhoto()}]");
        if (Country != null) sb.Append($", Country:{Country.Id ?? "-"}");
        if (Manager != null) sb.Append($", Manager:{Manager.Id ?? "-"}");
        if (Employees.Count != 0) sb.Append($", Employees:[{GetEmployees()}]");
        if (Talents.Count != 0) sb.Append($", Talents:[{GetTalents()}]");
        return sb.ToString();

        string GetEmployees() => string.Join(", ", _Employees.Select(x => x.Id ?? "-"));
        string GetTalents() => string.Join(", ", _Talents.Select(x => x.Id ?? "-"));
        string GetPhoto() => string.Join(", ", Photo!.Select(x => x));
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
    /// The name of this instance, or an empty instance.
    /// </summary>
    public EmployeeName Name
    {
        get => _Name;
        set => _Name = value.ThrowIfNull();
    }
    EmployeeName _Name = new();

    /// <summary>
    /// The employee birthdate, or null.
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// Whether this employee is an active one, or not.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// The photo of the employee.
    /// </summary>
    public byte[]? Photo { get; set; }

    /// <summary>
    /// The country this employee belongs to, or null.
    /// </summary>
    public Country? Country
    {
        get => _Country.Value;
        set => _Country.Value = value;
    }
    XParent<Employee, Country> _Country;

    /// <summary>
    /// The manager of this employee, or null.
    /// </summary>
    public Employee? Manager
    {
        get => _Manager.Value;
        set => _Manager.Value = value;
    }
    XParent<Employee, Employee> _Manager;

    /// <summary>
    /// The collection of employees that report into this one.
    /// </summary>
    public ICollection<Employee> Employees => _Employees;
    XChilds<Employee, Employee> _Employees;

    /// <summary>
    /// The collection of talents of this employee.
    /// </summary>
    public ICollection<Talent> Talents => _Talents;
    XTangled<Employee, Talent> _Talents;
}

// ========================================================
public class EmployeeName
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public EmployeeName() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    public EmployeeName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        FirstName == null ? LastName ?? "-" :
        LastName == null ? FirstName : $"{FirstName} {LastName}";

    /// <summary>
    /// The first name of this instance, or null.
    /// </summary>
    public string? FirstName
    {
        get => _FirstName;
        set => _FirstName = value?.NotNullNotEmpty();
    }
    string? _FirstName = null;

    /// <summary>
    /// The last name of this instance, or null.
    /// </summary>
    public string? LastName
    {
        get => _LastName;
        set => _LastName = value?.NotNullNotEmpty();
    }
    string? _LastName = null;
}