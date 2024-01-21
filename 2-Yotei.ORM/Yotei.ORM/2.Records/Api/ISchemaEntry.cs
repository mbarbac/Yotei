using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata that describes an entry of a given record.
/// </summary>
public interface ISchemaEntry : IEnumerable<TPair>
{
}