namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the records-oriented capabilities of a given connection.
/// </summary>
public interface IRecordsGate
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }
}