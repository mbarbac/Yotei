namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record-oriented command that, when executed against an underlying database,
/// produces an integer as the result of that execution.
/// </summary>
public interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandExecutor GetAsyncExecutor(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Executes this command and returns the integer produced by that execution.
    /// </summary>
    /// <returns></returns>
    int Execute();

    /// <summary>
    /// Executes this command and returns the integer produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteAsync(CancellationToken token = default);
}