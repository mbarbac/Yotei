namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command and obtains an integer as the
/// result of that execution.
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    ICommand Command { get; }

    /// <summary>
    /// Executes the associated command and returns the integer produced by that execution.
    /// </summary>
    /// <returns></returns>
    int Execute();

    /// <summary>
    /// Executes the associated command and returns the integer produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteAsync(CancellationToken token = default);
}