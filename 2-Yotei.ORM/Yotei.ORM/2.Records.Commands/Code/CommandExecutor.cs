namespace Yotei.ORM;

// ========================================================
/// <inheritdoc cref="ICommandExecutor"/>
public abstract class CommandExecutor : DisposableClass, ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public CommandExecutor(IExecutableCommand command) => Command = command.ThrowWhenNull();

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing) { }

    /// <inheritdoc/>
    protected override ValueTask OnDisposeAsync(bool disposing) => ValueTask.CompletedTask;

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Executor({Command})";

    /// <inheritdoc/>
    public IExecutableCommand Command { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
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
            if (openedByThis && !Command.Connection.IsOpen)
                Command.Connection.Close();
        }
    }

    /// <summary>
    /// Invoked to execute the associated command.
    /// </summary>
    /// <returns></returns>
    protected abstract int OnExecute();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public async ValueTask<int> ExecuteAsync(CancellationToken token = default)
    {
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
            if (openedByThis && !Command.Connection.IsOpen)
                await Command.Connection.CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Invoked to execute the associated command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask<int> OnExecuteAsync(CancellationToken token);
}