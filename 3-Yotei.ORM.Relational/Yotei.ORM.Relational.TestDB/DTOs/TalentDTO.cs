namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public class TalentDTO
{
    public string? Id { get; set; }
    public string? Description { get; set; }
    public object? RowVersion { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Id:{Id ?? "-"}");
        if (Description != null) sb.Append($", Description:{Description}");
        if (RowVersion != null) sb.Append($", RowVersion:{RowVersion.Sketch()}");
        return sb.ToString();
    }
}