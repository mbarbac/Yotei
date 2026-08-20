namespace Yotei.ORM.Relational.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
public class RecordsGate : ORM.Records.Code.RecordsGate, IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public RecordsGate(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc cref="IConnection"/>
    /// </summary>
    public new IConnection Connection => (IConnection)base.Connection;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public override ICommandEnumerator CreateEnumerator(
        IEnumerableCommand command, CancellationToken token = default) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public override ICommandExecutor CreateExecutor(IExecutableCommand command) => throw null;
}