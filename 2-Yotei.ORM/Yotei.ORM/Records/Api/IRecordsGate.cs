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

    /// <summary>
    /// Returns an object that can execute the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandEnumerator CommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// Returns an object that can execute the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandExecutor CommandExecutor(IExecutableCommand command);
}