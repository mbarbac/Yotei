namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the value and metadata of a given entry in a record.
/// </summary>
public interface IRecordEntry
{
    /// <summary>
    /// The value carried by this entry.
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// The metadata that describes this entry.
    /// </summary>
    ISchemaEntry Metadata { get; }
}