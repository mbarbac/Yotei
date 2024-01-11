namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command and enumerate the records
/// produced by that execution, if any.
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
    /// The schema that describes the records produced by the execution of the associated command,
    /// or null if it has not been executed yet.
    /// </summary>
    ISchema? Schema { get; }

    /// <summary>
    /// The record produced by the current iteration of this enumerator, or null if the command
    /// has not been executed yet, or if there are no more records available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to get
    /// the next available record.
    /// </summary>
    /// <returns></returns>
    new bool MoveNext();

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to get
    /// the next available record.
    /// </summary>
    /// <returns></returns>
    new ValueTask<bool> MoveNextAsync();
}