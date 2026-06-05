namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public class RegionDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ParentId { get; set; }
    public object? RowVersion { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        if (Name != null) sb.Append($", Name:{Name}");
        if (ParentId != null) sb.Append($", ParentId:{ParentId}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}