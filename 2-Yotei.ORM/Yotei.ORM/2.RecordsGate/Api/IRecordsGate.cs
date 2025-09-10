namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the record-oriented capabilities of the associated connection.
/// </summary>
public interface IRecordsGate
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute the given command, and enumerate through the records
    /// produced by that execution, if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);
}