namespace Kappa.Domain;

// ========================================================
/// <summary>
/// Represents a talent.
/// </summary>
public class Talent
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="id"></param>
    public Talent(string id)
    {
        Id = id;
        Employees = new(this);
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
        if (Employees.Count != 0) sb.Append($", Employees:{Employees.Select(x => x.Id).Sketch()}");
        return sb.ToString();
    }

    /// <summary>
    /// The unique id of this Talent.
    /// </summary>
    public string Id
    {
        get => _Id;
        init => _Id = value.NotNullNotEmpty();
    }
    string _Id = default!;

    /// <summary>
    /// The name of this Talent, or null.
    /// </summary>
    public string? Name
    {
        get => _Name;
        init => _Name = value?.NotNullNotEmpty();
    }
    string? _Name = default!;

    /// <summary>
    /// The list of employees with this talent.
    /// </summary>
    public EmployeeList Employees { get; }
    public class EmployeeList : CustomList<Employee>
    {
        readonly Talent Master;
        internal EmployeeList(Talent master)
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
            if (r > 0) item.Talents.Add(Master);
            return r;
        }
        public override int Insert(int index, Employee item)
        {
            if (Contains(x => ReferenceEquals(x, item))) return 0;

            var r = base.Insert(index, item);
            if (r > 0) item.Talents.Add(Master);
            return r;
        }
        public override int RemoveAt(int index)
        {
            var item = this[index];
            var r = base.RemoveAt(index);
            if (r > 0) item.Talents.Remove(Master);
            return r;
        }
    }
}