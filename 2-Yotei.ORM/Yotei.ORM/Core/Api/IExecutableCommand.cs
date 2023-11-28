namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a command that when executed returns an integer as the result of that execution.
/// </summary>
public interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

    /// <summary>
    /// Executes this command and returns the integer produced as the result of that execution.
    /// </summary>
    /// <returns></returns>
    int Execute();

    /// <summary>
    /// Executes this command and returns the integer produced as the result of that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteAsync(CancellationToken token = default);
}