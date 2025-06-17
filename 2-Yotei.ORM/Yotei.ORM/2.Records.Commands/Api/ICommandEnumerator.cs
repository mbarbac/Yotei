namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated enumerable command and enumerate the
/// records produced by that execution, if any.
/// </summary>
public interface ICommandEnumerator
    : IDisposableEx
    , IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>
    , IEnumerable<IRecord?>, IAsyncEnumerable<IRecord?>
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token used by this instance.
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Gets the record produced by the current iteration of the execution of the associated
    /// command, or <c>null</c> if it has not been executed yet, or if there are no more results
    /// available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// The schema that describes the records produced by this instance, or <c>null</c> if the
    /// associated command has not been executed yet, or if this information is not available.
    /// </summary>
    ISchema? Schema { get; }
}