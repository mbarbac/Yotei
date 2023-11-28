namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that can execute an executable command and return an integer as the
/// result of that execution.
/// </summary>
public interface ICommandExecutor : IBaseDisposable
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IExecutableCommand Command { get; }

    /// <summary>
    /// Executes the associated command and returns the integer produced as the result of that
    /// execution.
    /// </summary>
    /// <returns></returns>
    int Execute();

    /// <summary>
    /// Executes the associated command and returns the integer produced as the result of that
    /// execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteAsync(CancellationToken token = default);
}