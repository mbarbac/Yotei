namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public class EmployeeDTO
{
    public string? Id { get; set; } = null;
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public DateOnly? BirthDate { get; set; } = null;
    public bool? Active { get; set; } = null;
    public DateOnly? JoinDate { get; set; } = null;
    public TimeOnly? StartTime { get; set; } = null;
    public byte[]? Photo { get; set; } = null;
    public string? CountryId { get; set; } = null;
    public string? ManagerId { get; set; } = null;
    public object? RowVersion { get; set; } = null;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        if (FirstName != null) sb.Append($", FirstName:{FirstName}");
        if (LastName != null) sb.Append($", LastName:{LastName}");
        if (BirthDate != null) sb.Append($", BirthDate:{BirthDate}");
        if (Active != null) sb.Append($", Active:{Active.Value}");
        if (JoinDate != null) sb.Append($", JoinDate:{JoinDate.Value.Day}.{JoinDate.Value.Month}.{JoinDate.Value.Year}");
        if (StartTime != null) sb.Append($", StartTime:{StartTime.Value.Hour}.{StartTime.Value.Minute}.{StartTime.Value.Second}");
        if (Photo != null) sb.Append($", Photo:{Photo.Sketch()}");
        if (CountryId != null) sb.Append($", CountryId:{CountryId}");
        if (ManagerId != null) sb.Append($", ManagerId:{ManagerId}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}