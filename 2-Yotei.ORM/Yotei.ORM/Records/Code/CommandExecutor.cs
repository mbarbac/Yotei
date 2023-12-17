namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandExecutor"/>
/// </summary>
public abstract class CommandExecutor : ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public CommandExecutor(ICommand command) => Command = command.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Executor({Command})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ICommand Command { get; }
    ORM.ICommand ORM.ICommandExecutor.Command => Command;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int Execute()
    {
        bool openedByThis = false;
        try
        {
            if (!Command.Connection.IsOpen)
            {
                Command.Connection.Open();
                openedByThis = true;
            }
            return OnExecute();
        }
        finally
        {
            if (openedByThis && Command.Connection.IsOpen)
                Command.Connection.Close();
        }
    }

    /// <summary>
    /// Invoked to execute the command.
    /// </summary>
    /// <returns></returns>
    protected abstract int OnExecute();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        bool openedByThis = false;
        try
        {
            if (!Command.Connection.IsOpen)
            {
                await Command.Connection.OpenAsync(token).ConfigureAwait(false);
                openedByThis = true;
            }
            return await OnExecuteAsync(token).ConfigureAwait(false);
        }
        finally
        {
            if (openedByThis && Command.Connection.IsOpen)
                await Command.Connection.CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Invoked to execute the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask<int> OnExecuteAsync(CancellationToken token);
}