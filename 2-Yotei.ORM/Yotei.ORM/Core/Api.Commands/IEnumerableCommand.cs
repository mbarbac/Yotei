namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a command that when executed enumerates the result that execution produces.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEnumerableCommand<T> : ICommand, IEnumerable<T>, IAsyncEnumerable<T>
    where T : class
{
    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator<T> GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator<T> GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this command can perform native paging, or rather it shall be emulated by
    /// the framework.
    /// </summary>
    bool NativePaging { get; }

    /// <summary>
    /// The number of results to skip before returning the remaining ones.
    /// <br/> If the value of this property is less than cero, then it is ignored.
    /// </summary>
    int Skip { get; set; }

    /// <summary>
    /// The number of results to enumerate at most.
    /// <br/> If the value of this property is less than cero, then it is ignored.
    /// </summary>
    int Take { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Executes this command and returns a list with the results produced from that execution.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Executes this command and returns a list with the results produced from that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<List<T>> ToListAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns an array with the results produced from that execution.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Executes this command and returns an array with the results produced from that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<T[]> ToArrayAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns the first result produced, or null if any.
    /// </summary>
    /// <returns></returns>
    T? First();

    /// <summary>
    /// Executes this command and returns the first result produced, or null if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<T?> FirstAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns the last result produced, or null if any.
    /// <br/> This method is provided as a fall-back mechanism. It enumerates all the results
    /// discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    T? Last();

    /// <summary>
    /// Executes this command and returns the last result produced, or null if any.
    /// <br/> This method is provided as a fall-back mechanism. It enumerates all the results
    /// discarding them until the last one is found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<T?> LastAsync(CancellationToken token = default);
}