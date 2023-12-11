namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents an employee.
/// </summary>
public class Employee
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    public Employee(string id, string? first = null, string? last = null)
    {
        Id = id;
        Name = new(first, last);
        Employees = new(this);
        Talents = new(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Id);
        if (Name.First != null || Name.Last != null) sb.Append($", Name:{Name}");
        if (Active != null) sb.Append($", Active:{Active}");
        if (Country != null) sb.Append($", Country:{Country.Id}");
        if (Manager != null) sb.Append($", Manager:{Manager.Id}");
        if (Employees.Count != 0) sb.Append($", Countries:{Employees.Select(x => x.Id).Sketch()}");
        if (Talents.Count != 0) sb.Append($", Talents:{Talents.Select(x => x.Id).Sketch()}");
        if (BirthDate != null) sb.Append($", BirthDate:{BirthDate}");
        if (Photo != null) sb.Append($", Photo:[{Photo.Sketch()}]");
        return sb.ToString();
    }

    /// <summary>
    /// The unique id of this Employee.
    /// </summary>
    public string Id
    {
        get => _Id;
        init => _Id = value.NotNullNotEmpty();
    }
    string _Id = default!;

    /// <summary>
    /// The first and last names of this employee.
    /// </summary>
    public EmployeeName Name { get; }

    /// <summary>
    /// Whether the employee is active, or not, or null.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// The country this employee belong to, or null.
    /// </summary>
    public Country? Country
    {
        get => _Country;
        set
        {
            if (_Country == value) return;

            _Country?.Employees.Remove(this);
            _Country = value;
            _Country?.Employees.Add(this);
        }
    }
    Country? _Country;

    /// <summary>
    /// The manager of this employee, or null.
    /// </summary>
    public Employee? Manager
    {
        get => _Manager;
        set
        {
            if (_Manager == value) return;

            _Manager?.Employees.Remove(this);
            _Manager = value;
            _Manager?.Employees.Add(this);
        }
    }
    Employee? _Manager;

    /// <summary>
    /// The list of employees that report to this one.
    /// </summary>
    public EmployeeList Employees { get; }
    public class EmployeeList : CustomList<Employee>
    {
        readonly Employee Master;
        internal EmployeeList(Employee master)
        {
            Master = master;
            Validate = (item, add) => item.ThrowWhenNull();
            Compare = (source, target) => source.Id == target.Id;
            AcceptDuplicate = (source, target)
                => ReferenceEquals(source, target)
                ? false
                : throw new ArgumentException("Duplicate item.").WithData(target);
        }
        protected override string Item2String(Employee item) => item.Id;
        public override int Add(Employee item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Add(item);
            if (r > 0) item.Manager = Master;
            return r;
        }
        public override int Insert(int index, Employee item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Insert(index, item);
            if (r > 0) item.Manager = Master;
            return r;
        }
        public override int RemoveAt(int index)
        {
            var item = this[index];
            var r = base.RemoveAt(index);
            if (r > 0) item.Employees.Remove(Master);
            return r;
        }
    }

    /// <summary>
    /// The talents of this employee.
    /// </summary>
    public TalentList Talents { get; }
    public class TalentList : CustomList<Talent>
    {
        readonly Employee Master;
        internal TalentList(Employee master)
        {
            Master = master;
            Validate = (item, add) => item.ThrowWhenNull();
            Compare = (source, target) => source.Id == target.Id;
            AcceptDuplicate = (source, target)
                => ReferenceEquals(source, target)
                ? false
                : throw new ArgumentException("Duplicate item.").WithData(target);
        }
        protected override string Item2String(Talent item) => item.Id;
        public override int Add(Talent item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Add(item);
            if (r > 0) item.Employees.Add(Master);
            return r;
        }
        public override int Insert(int index, Talent item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Insert(index, item);
            if (r > 0) item.Employees.Add(Master);
            return r;
        }
        public override int RemoveAt(int index)
        {
            var item = this[index];
            var r = base.RemoveAt(index);
            if (r > 0) item.Employees.Remove(Master);
            return r;
        }
    }

    /// <summary>
    /// The employee birthdate, or null.
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// The photo of the employee.
    /// </summary>
    public byte[]? Photo { get; set; }

    // ====================================================
    public class EmployeeName(string? first, string? last)
    {
        public string? First { get; set; } = first?.NotNullNotEmpty();
        public string? Last { get; set; } = last?.NotNullNotEmpty();
        public override string ToString() =>
            First == null ? Last ?? "" :
            Last == null ? First : $"{First} {Last}";
    }
}