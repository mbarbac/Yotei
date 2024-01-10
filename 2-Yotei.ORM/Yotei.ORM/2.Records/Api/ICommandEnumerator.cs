namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command and enumerate the results produced
/// by that execution.
/// </summary>
public interface ICommandEnumerator
    : IBaseDisposable
    , IEnumerator<IRecord?>
    , IAsyncEnumerator<IRecord?>
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token used by this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The schema that describes the records produced by the execution of the associate command,
    /// or null if it has not been executed yet.
    /// </summary>
    ISchema? Schema { get; }

    /// <summary>
    /// The record produced by the current iteration of the execution of the associated command,
    /// or null if it has not been executed yet, or no more records are available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to obtain
    /// the next record resulting from that execution.
    /// </summary>
    /// <returns></returns>
    new bool MoveNext();

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to obtain
    /// the next record resulting from that execution.
    /// </summary>
    /// <returns></returns>
    new ValueTask<bool> MoveNextAsync();
}