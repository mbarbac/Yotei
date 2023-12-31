namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute a records-oriented executable command and return the
/// integer produced by that execution.
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IExecutableCommand Command { get; }

    /// <summary>
    /// Executes the associated command and return the integer produced by that execution.
    /// </summary>
    /// <returns></returns>
    int Execute();

    /// <summary>
    /// Executes the associated command and return the integer produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteAsync(CancellationToken token = default);
}