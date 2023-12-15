namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public class EmployeeTalentDTO
{
    public string? EmployeeId { get; set; } = null;
    public string? TalentId { get; set; } = null;
    public object? RowVersion { get; set; } = null;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"EmployeeId:{EmployeeId ?? "-"}");
        sb.Append($", TalentId:{TalentId ?? "-"}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}