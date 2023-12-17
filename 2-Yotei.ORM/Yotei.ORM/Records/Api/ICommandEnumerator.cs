namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ICommandEnumerator"/>
/// </summary>
public interface ICommandEnumerator
    : ORM.ICommandEnumerator, IEnumerator<IRecord>, IAsyncEnumerator<IRecord>
{
    /// <summary>
    /// <inheritdoc cref="ICommandEnumerator.ICommand"/>
    /// </summary>
    new ICommand Command { get; }

    /// <summary>
    /// The cancellation token given to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Determines if this instance supports native paging, or not.
    /// </summary>
    bool NativePaging { get; set; }

    /// <summary>
    /// The number of records to skip, or cero if not enabled.
    /// </summary>
    int Skip { get; set; }

    /// <summary>
    /// The number of record to take by page, or cero if not enabled.
    /// </summary>
    int Take { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// The record produced by the current iteration of the execution of the command, or null
    /// if it has not been executed yet, or if there are no more records available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// The schema of the records produced by the execution of the command, or null if it has
    /// not been executed yet.
    /// </summary>
    ISchema? Schema { get; }
}