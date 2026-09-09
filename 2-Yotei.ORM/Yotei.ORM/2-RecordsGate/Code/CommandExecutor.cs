using System.Runtime.InteropServices.Marshalling;

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
    [SuppressMessage("", "IDE0290")]
    public CommandExecutor(IExecutableCommand command) => Command = command.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IExecutableCommand Command { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int Execute()
    {
        var connection = Command.Connection;
        bool openedByThis = false;
        try
        {
            if (!connection.IsOpen)
            {
                connection.Open();
                openedByThis = true;
            }

            return OnExecute();
        }
        finally
        {
            if (openedByThis && connection.IsOpen) connection.Close();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync(CancellationToken token = default)
    {
        var connection = Command.Connection;
        bool openedByThis = false;
        try
        {
            token.ThrowIfCancellationRequested();

            if (!connection.IsOpen)
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                openedByThis = true;
            }

            return await OnExecuteAsync(token).ConfigureAwait(false);
        }
        finally
        {
            if (openedByThis && connection.IsOpen)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to execute the associated command.
    /// </summary>
    /// <returns></returns>
    protected abstract int OnExecute();

    /// <summary>
    /// Invoked to execute the associated command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask<int> OnExecuteAsync(CancellationToken token);
}