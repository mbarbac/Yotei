namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public class CountryDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? RegionId { get; set; }
    public object? RowVersion { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        if (Name != null) sb.Append($", Name:{Name}");
        if (RegionId != null) sb.Append($", RegionId:{RegionId}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}