namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides acces to the record-oriented capabilities of a given connection.
/// </summary>
public interface IRecordOperations
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Returns a new enumerator for the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandEnumerator CreateEnumerator(IEnumerableCommand command);

    /// <summary>
    /// Returns a new enumerator for the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandEnumerator CreateAsyncEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// Returns a new executor for the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandExecutor CreateExecutor(IExecutableCommand command);

    /// <summary>
    /// Returns a new executor for the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandExecutor CreateAsyncExecutor(IExecutableCommand command, CancellationToken token = default);
}