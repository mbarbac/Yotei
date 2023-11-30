namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that can execute an enumerable command and enumerate the results that
/// execution produces.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandEnumerator<T> : IEnumerator<T>, IAsyncEnumerator<T>, IBaseDisposable
    where T : class
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand<T> Command { get; }

    /// <summary>
    /// The cancellation token given to this enumerator.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the result produced by the current iteration of this instance, or null if no more
    /// results are available, or if the command has not been executed yet.
    /// </summary>
    new T? Current { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the associated command and returns a list with the results produced from that
    /// execution.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Executes the associated command and returns a list with the results produced from that
    /// execution.
    /// </summary>
    /// <returns></returns>
    ValueTask<List<T>> ToListAsync();

    /// <summary>
    /// Executes the associated command and returns an array with the results produced from that
    /// execution.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Executes the associated command and returns an array with the results produced from that
    /// execution.
    /// </summary>
    /// <returns></returns>
    ValueTask<T[]> ToArrayAsync();

    /// <summary>
    /// Executes the associated command and returns the first result produced, or null if any.
    /// </summary>
    /// <returns></returns>
    T? First();

    /// <summary>
    /// Executes the associated command and returns the first result produced, or null if any.
    /// </summary>
    /// <returns></returns>
    ValueTask<T?> FirstAsync();

    /// <summary>
    /// Executes the associated command and returns the last result produced, or null if any.
    /// <br/> This method is provided as a fall-back mechanism. It enumerates all the results
    /// discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    T? Last();

    /// <summary>
    /// Executes the associated command and returns the last result produced, or null if any.
    /// <br/> This method is provided as a fall-back mechanism. It enumerates all the results
    /// discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    ValueTask<T?> LastAsync();
}