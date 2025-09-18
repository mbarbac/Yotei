namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
public abstract class RecordsGate : IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <inheritdoc/>
    public IConnection Connection { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default);

    /// <inheritdoc/>
    public abstract ICommandExecutor CreateCommandExecutor(IExecutableCommand command);
}