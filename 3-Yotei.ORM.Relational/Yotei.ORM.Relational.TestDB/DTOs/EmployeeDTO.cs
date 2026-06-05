namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public class EmployeeDTO
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public bool? Active { get; set; }
    public DateOnly? JoinDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public byte[]? Photo { get; set; }
    public string? CountryId { get; set; }
    public string? ManagerId { get; set; }
    public object? RowVersion { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        
        if (FirstName != null) sb.Append($", FirstName:{FirstName}");
        if (LastName != null) sb.Append($", LastName:{LastName}");
        if (BirthDate != null) sb.Append($", BirthDate:{BirthDate.Value.ToIsoString()}");
        if (Active != null) sb.Append($", Active:{Active.Value}");
        if (JoinDate != null) sb.Append($", JoinDate:{JoinDate.Value.ToIsoString()}");
        if (StartTime != null) sb.Append($", StartTime:{StartTime.Value.ToIsoString()}");
        if (Photo != null) sb.Append($", Photo:{Photo.Sketch()}");
        if (CountryId != null) sb.Append($", CountryId:{CountryId}");
        if (ManagerId != null) sb.Append($", ManagerId:{ManagerId}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}