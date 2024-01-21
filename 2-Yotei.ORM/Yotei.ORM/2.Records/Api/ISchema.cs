using TItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata that describes the structure and contents of a given record.
/// </summary>
public interface ISchema : IFrozenList<TKey, TItem>
{
}