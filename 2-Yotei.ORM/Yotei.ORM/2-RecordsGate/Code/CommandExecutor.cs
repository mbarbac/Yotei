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
    public CommandExecutor(IExecutableCommand command)
    {
        Command = command.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    protected override void OnDispose(bool disposing) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected override ValueTask OnDisposeAsync(bool disposing) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IExecutableCommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract int Execute();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ValueTask<int> ExecuteAsync(CancellationToken token = default);
}