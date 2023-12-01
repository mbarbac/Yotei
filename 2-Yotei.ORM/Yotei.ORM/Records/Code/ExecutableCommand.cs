namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IExecutableCommand"/>
/// </summary>
public abstract class ExecutableCommand : Command, IExecutableCommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public ExecutableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateExecutor(this);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandExecutor GetAsyncExecutor(
        CancellationToken token = default) => Connection.Records.CreateAsyncExecutor(this, token);

    // ----------------------------------------------------

    /// <summary>
    /// Executes this command and returns the integer produced by that execution.
    /// </summary>
    /// <returns></returns>
    public int Execute()
    {
        using var iter = GetExecutor();
        return iter.Execute();
    }

    /// <summary>
    /// Executes this command and returns the integer produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask<int> ExecuteAsync(CancellationToken token = default)
    {
        await using var iter = GetAsyncExecutor(token);
        return await iter.ExecuteAsync().ConfigureAwait(false);
    }
}