namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordOperations"/>
/// </summary>
public abstract class RecordOperations : IRecordOperations
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordOperations(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator CreateEnumerator(IEnumerableCommand command);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator CreateAsyncEnumerator(
        IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public abstract ICommandExecutor CreateExecutor(IExecutableCommand command);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandExecutor CreateAsyncExecutor(
        IExecutableCommand command, CancellationToken token = default);
}