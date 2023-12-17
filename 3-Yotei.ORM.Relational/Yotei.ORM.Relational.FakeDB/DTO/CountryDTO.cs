namespace Yotei.ORM.Relational.FakeDB.DTO;

// ========================================================
public class CountryDTO
{
    public string? Id { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? RegionId { get; set; } = null;
    public object? RowVersion { get; set; } = null;

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