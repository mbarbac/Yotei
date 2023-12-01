namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record-oriented command that, when executed against an underlying database,
/// produces a set or records as the result of that execution.
/// </summary>
public interface IEnumerableCommand : ICommand, IEnumerable<IRecord>, IAsyncEnumerable<IRecord>
{
    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this command supports native paging capabilities, or rather they have to
    /// be emulated by the framework.
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
    /// Executes this command and returns a list with the records produced by that execution.
    /// </summary>
    /// <returns></returns>
    List<IRecord> ToList();

    /// <summary>
    /// Executes this command and returns a list with the records produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<List<IRecord>> ToListAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns an array with the records produced by that execution.
    /// </summary>
    /// <returns></returns>
    IRecord[] ToArray();

    /// <summary>
    /// Executes this command and returns an array with the records produced by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<IRecord[]> ToArrayAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns the first record produced by that execution, or null
    /// if any was produced.
    /// </summary>
    /// <returns></returns>
    IRecord? First();

    /// <summary>
    /// Executes this command and returns the first record produced by that execution, or null
    /// if any was produced.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<IRecord?> FirstAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns the last record produced by that execution, or null
    /// if any was produced.
    /// <br/> This method is provided as a fallback mechanism. It works by retrieving all records
    /// discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    IRecord? Last();

    /// <summary>
    /// Executes this command and returns the last record produced by that execution, or null
    /// if any was produced.
    /// <br/> This method is provided as a fallback mechanism. It works by retrieving all records
    /// discarding them until the last one is found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<IRecord?> LastAsync(CancellationToken token = default);
}