namespace Yotei.ORM.Relational;

// ========================================================
/// <inheritdoc cref="ORM.IRecordsGate"/>
public interface IRecordsGate : ORM.IRecordsGate
{
    /// <inheritdoc cref="ORM.IRecordsGate.Connection"/>
    new IConnection Connection { get; }

    // ----------------------------------------------------

    /// <inheritdoc cref="ORM.IRecordsGate.CreateCommandEnumerator(IEnumerableCommand, CancellationToken)"/>
    new ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <inheritdoc cref="ORM.IRecordsGate.CreateCommandExecutor(IExecutableCommand)"/>
    new ICommandExecutor CreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a database command for the contents captured by the given records' one.
    /// <br/> This method is INFRASTRUCTURE and it is NOT intended to be used by application code.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    DbCommand CreateDbCommand(ICommand command, bool iterable);

    /// <summary>
    /// Returns a database command for the contents captured by the given records' one.
    /// <br/> This method is INFRASTRUCTURE and it is NOT intended to be used by application code.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    void AddTransaction(DbCommand command);
}