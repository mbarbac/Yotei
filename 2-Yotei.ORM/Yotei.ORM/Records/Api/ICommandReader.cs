namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented forward-only reader of the data produced by an enumerable
/// command.
/// </summary>
public interface ICommandReader : ORM.ICommandReader, IEnumerable<IRecord>
{
    /// <summary>
    /// The schema that describes the structure and contents of the records produced by the
    /// execution of the associated command.
    /// </summary>
    ISchema Schema { get; }
}