namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the records produced
/// by that execution, if any.
/// </summary>
public interface ICommandEnumerator
    : IDisposableEx
    , IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>
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
    /// The schema that describes the records produced by this instance, or <c>null</c> if the
    /// associated command has not been executed yet, or if this information is not available.
    /// </summary>
    ISchema? Schema { get; }

    /// <summary>
    /// Gets the result produced by the current iteration, or <c>null</c> if the command has
    /// not been executed yet, or if there are no more results available.
    /// </summary>
    new IRecord? Current { get; }
}