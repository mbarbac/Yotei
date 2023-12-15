namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a forward-only reader of the data produced by an enumerable command.
/// </summary>
public interface ICommandReader : IEnumerable<object>, IAsyncEnumerable<object>, IBaseDisposable
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The text of the command executed against the underlying database.
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// The ordered collection of parameters used by the command when executed.
    /// </summary>
    IParameterList Parameters { get; }

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