namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary command that can be executed against an underlying database.
/// </summary>
public interface ICommand : IEnumerable, IAsyncEnumerable<object>
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database,
    /// using the default iterable mode of this command.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database,
    /// either for an iterable environment or not.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute this command and enumerate through the results that
    /// execution produces.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate through the results that
    /// execution produces.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    /// <summary>
    /// Returns an object that can execute this command and produce an integer as the result of
    /// that execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the results produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    List<object> ToList();

    /// <summary>
    /// Returns a list with the results produced by the execution of the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<List<object>> ToListAsync(CancellationToken token = default);

    /// <summary>
    /// Returns an array with the results produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    object[] ToArray();

    /// <summary>
    /// Returns an array with the results produced by the execution of the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<object[]> ToArrayAsync(CancellationToken token = default);

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <returns></returns>
    object? First();

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<object?> FirstAsync(CancellationToken token = default);

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// results produced, discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    object? Last();

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// results produced, discarding them until the last one is found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<object?> LastAsync(CancellationToken token = default);
}