namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented forward-only reader of the data produced by an enumerable
/// command.
/// </summary>
public interface ICommandReader : ORM.ICommandReader, IEnumerable<IRecord>, IAsyncEnumerable<IRecord>
{
    /// <summary>
    /// The schema that describes the structure and contents of the records produced by the
    /// execution of the associated command.
    /// </summary>
    ISchema Schema { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    new List<IRecord> ToList();

    /// <summary>
    /// Returns a list with the records produced by the execution of the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<List<IRecord>> ToListAsync(CancellationToken token = default);

    /// <summary>
    /// Returns an array with the records produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    new IRecord[] ToArray();

    /// <summary>
    /// Returns an array with the records produced by the execution of the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<IRecord[]> ToArrayAsync(CancellationToken token = default);

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <returns></returns>
    new IRecord? First();

    /// <summary>
    /// Returns the first result produced by the execution of the command, or null if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<IRecord?> FirstAsync(CancellationToken token = default);

    /// <summary>
    /// Returns the last record produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// records produced, discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    new IRecord? Last();

    /// <summary>
    /// Returns the last record produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// records produced, discarding them until the last one is found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<IRecord?> LastAsync(CancellationToken token = default);
}