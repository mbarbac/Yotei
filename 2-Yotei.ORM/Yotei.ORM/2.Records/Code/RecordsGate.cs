namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
/// <param name="connection"
public abstract class RecordsGate(IConnection connection) : IRecordsGate
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; } = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public abstract ICommandExecutor CreateCommandExecutor(IExecutableCommand command);
}