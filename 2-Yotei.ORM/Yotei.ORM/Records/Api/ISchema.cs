namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the collection of metadata that describes the contents
/// and structure of a given record.
/// </summary>
public interface ISchema : IEnumerable<ISchemaEntry>
{
}