namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute a record-oriented executable command and return the
/// integer that execution produces.
/// </summary>
public interface ICommandExecutor : IBaseDisposable
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IExecutableCommand Command { get; }

    /// <summary>
    /// The cancellation token given to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

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
    /// <returns></returns>
    ValueTask<int> ExecuteAsync();
}