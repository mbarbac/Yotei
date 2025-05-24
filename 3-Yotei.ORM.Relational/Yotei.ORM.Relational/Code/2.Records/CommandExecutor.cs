namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="ICommandExecutor"/>
public class CommandExecutor : ORM.Code.CommandExecutor, ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public CommandExecutor(IExecutableCommand command) : base(command)
    {
        if (command.Connection is not IConnection)
            throw new ArgumentException(
                "Command's connection is not a relational one.")
                .WithData(command)
                .WithData(command.Connection);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Relational.Executor({Command})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override int OnExecute() => throw null;

    /// <inheritdoc/>
    protected override ValueTask<int> OnExecuteAsync(CancellationToken token) => throw null;
}