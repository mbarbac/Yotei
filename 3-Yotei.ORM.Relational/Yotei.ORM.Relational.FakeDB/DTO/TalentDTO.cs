namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public class TalentDTO
{
    public string? Id { get; set; } = null;
    public string? Description { get; set; } = null;
    public object? RowVersion { get; set; } = null;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        if (Description != null) sb.Append($", Name:{Description}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}