namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
/// <param name="connection"></param>
public abstract class RecordsGate(IConnection connection) : IRecordsGate
{
    /// <inheritdoc/>
    public IConnection Connection { get; } = connection.ThrowWhenNull();

    /// <inheritdoc/>
    public ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        if (Connection != command.Connection) throw new ArgumentException(
            "Connection of the given command is not the one of this instance.")
            .WithData(command)
            .WithData(Connection);

        return DoCreateCommandEnumerator(command, token);
    }

    /// <summary>
    /// Invoked to create a command enumerator.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ICommandEnumerator DoCreateCommandEnumerator(IEnumerableCommand command, CancellationToken token);

    /// <inheritdoc/>
    public ICommandExecutor CreateCommandExecutor(IExecutableCommand command)
    {
        command.ThrowWhenNull();

        if (Connection != command.Connection) throw new ArgumentException(
            "Connection of the given command is not the one of this instance.")
            .WithData(command)
            .WithData(Connection);

        return DoCreateCommandExecutor(command);
    }

    /// <summary>
    /// Invoked to create a command executor.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    protected abstract ICommandExecutor DoCreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Raw() => new RawCommand(Connection);

    /// <inheritdoc/>
    public virtual IRawCommand Raw(string? specs, params object?[] args) => new RawCommand(Connection, specs, args);

    /// <inheritdoc/>
    public virtual IRawCommand Raw(Func<dynamic, object> specs) => new RawCommand(Connection, specs);
}