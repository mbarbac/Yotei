namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed, returns a set of records as the
/// result of that execution.
/// </summary>
public interface IEnumerableCommand : ICommand, IEnumerable<IRecord?>, IAsyncEnumerable<IRecord?>
{
    /// <summary>
    /// Returns an enumerator that iterates through the results produced by the execution of this
    /// command.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates asynchronously through the results produced by the
    /// execution of this command..
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if this instance supports native paging, or not.
    /// </summary>
    bool NativePaging { get; }

    /// <summary>
    /// The number of records to skip before returning the first result, if any is left. A value
    /// less than cero means this setting is not enabled.
    /// </summary>
    int Skip { get; set; }

    /// <summary>
    /// The number of records to include in the set of results. A value less than cero means this
    /// setting is not enabled.
    /// </summary>
    int Take { get; set; }
}