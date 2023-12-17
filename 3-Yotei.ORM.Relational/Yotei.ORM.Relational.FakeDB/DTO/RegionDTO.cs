namespace Yotei.ORM.Relational.FakeDB.DTO;

// ========================================================
public class RegionDTO
{
    public class Region
    {
        public string? Id { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? ParentId { get; set; } = null;
        public object? RowVersion { get; set; } = null;

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
}