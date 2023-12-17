namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that can be executed agains an underlying database.
/// </summary>
public interface ICommand : ORM.ICommand, IEnumerable<IRecord>, IAsyncEnumerable<IRecord>
{
    /// <summary>
    /// <inheritdoc cref="ORM.ICommand.GetEnumerator"/>
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// <inheritdoc cref="ORM.ICommand.GetAsyncEnumerator(CancellationToken)"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    /// <summary>
    /// <inheritdoc cref="ORM.ICommand.GetExecutor"/>
    /// </summary>
    /// <returns></returns>
    new ICommandExecutor GetExecutor();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the results produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    new List<IRecord> ToList();

    /// <summary>
    /// Returns a list with the results produced by the execution of the command.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<List<IRecord>> ToListAsync(CancellationToken token = default);

    /// <summary>
    /// Returns an array with the results produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    new IRecord[] ToArray();

    /// <summary>
    /// Returns an array with the results produced by the execution of the command.
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
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// results produced, discarding them until the last one is found.
    /// </summary>
    /// <returns></returns>
    new IRecord? Last();

    /// <summary>
    /// Returns the last result produced by the execution of the command, or null if any.
    /// <br/> This method is provided as a fallback mechanism, as it iterates through all the
    /// results produced, discarding them until the last one is found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<IRecord?> LastAsync(CancellationToken token = default);
}