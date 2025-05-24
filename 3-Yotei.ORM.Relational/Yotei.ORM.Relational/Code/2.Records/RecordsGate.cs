namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
public class RecordsGate : ORM.Code.RecordsGate, IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) : base(connection) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IConnection Connection => (IConnection)base.Connection;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        if (!ReferenceEquals(Connection, command.Connection))
            throw new InvalidOperationException(
                "Command's connection is not the same as this instance's one.")
                .WithData(command)
                .WithData(this);

        return new CommandEnumerator(command, token);
    }

    /// <inheritdoc/>
    public override ICommandExecutor CreateCommandExecutor(
        IExecutableCommand command)
    {
        command.ThrowWhenNull();

        if (!ReferenceEquals(Connection, command.Connection))
            throw new InvalidOperationException(
                "Command's connection is not the same as this instance's one.")
                .WithData(command)
                .WithData(this);

        return new CommandExecutor(command);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual DbCommand CreateDbCommand(ICommand command, bool iterable) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void AddTransaction(DbCommand command) => throw null;
}