namespace Yotei.ORM.Relational.Records;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.Records.IRecordsGate"/>
/// </summary>
public interface IRecordsGate : ORM.Records.IRecordsGate
{
    /// <summary>
    /// <inheritdoc cref="Records.IRecordsGate.Connection"/>
    /// </summary>
    new IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Records.IRecordsGate.CreateEnumerator(IEnumerableCommand, CancellationToken)"/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator CreateEnumerator(
        IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// <inheritdoc cref="Records.IRecordsGate.CreateExecutor(IExecutableCommand)"/>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    new ICommandExecutor CreateExecutor(IExecutableCommand command);
}