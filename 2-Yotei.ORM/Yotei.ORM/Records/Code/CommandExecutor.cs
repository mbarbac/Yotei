namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command and return the integer produced
/// by that execution.
/// </summary>
public abstract class CommandExecutor : ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    [SuppressMessage("", "IDE0290")]
    public CommandExecutor(IExecutableCommand command) => Command = command.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Executor({Command})";

    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    public IExecutableCommand Command { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the associated command, using its current state, and returns the integer produced
    /// by that execution.
    /// </summary>
    /// <returns></returns>
    public int Execute()
    {
        bool openByThis = false;
        try
        {
            if (!Command.Connection.IsOpen)
            {
                Command.Connection.Open();
                openByThis = true;
            }
            return OnExecute();
        }
        finally
        {
            if (openByThis &&
                Command.Connection.IsOpen)
                Command.Connection.Close();
        }
    }

    /// <summary>
    /// Executes the associated command.
    /// </summary>
    /// <returns></returns>
    protected abstract int OnExecute();

    // ----------------------------------------------------

    /// <summary>
    /// Executes the associated command, using its current state, and returns the integer produced
    /// by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync(CancellationToken token = default)
    {
        bool openByThis = false;
        try
        {
            if (!Command.Connection.IsOpen)
            {
                await Command.Connection.OpenAsync(token).ConfigureAwait(false);
                openByThis = true;
            }
            return await OnExecuteAsync(token).ConfigureAwait(false);
        }
        finally
        {
            if (openByThis &&
                Command.Connection.IsOpen)
                await Command.Connection.CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes the associated command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask<int> OnExecuteAsync(CancellationToken token);
}