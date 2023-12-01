namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandExecutor"/>
/// </summary>
public abstract class CommandExecutor : DisposableClass, ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandExecutor(IExecutableCommand command, CancellationToken token = default)
    {
        Command = command.ThrowWhenNull();
        CancellationToken = token;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override ValueTask OnDisposeAsync(bool disposing) => ValueTask.CompletedTask;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"ORM.Executor[{Command}]";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IExecutableCommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int Execute()
    {
        CancellationToken.ThrowIfCancellationRequested();

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
            try { if (openedByThis && Command.Connection.IsOpen) Command.Connection.Close(); }
            catch { }
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
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync()
    {
        CancellationToken.ThrowIfCancellationRequested();

        bool openedByThis = false;
        try
        {
            if (!Command.Connection.IsOpen)
            {
                await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
                openedByThis = true;
            }
            return await OnExecuteAsync().ConfigureAwait(false);
        }
        finally
        {
            try
            {
                if (openedByThis && Command.Connection.IsOpen)
                    await Command.Connection.CloseAsync().ConfigureAwait(false);
            }
            catch { }
        }
    }

    /// <summary>
    /// Invoked to execute the command.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<int> OnExecuteAsync();
}