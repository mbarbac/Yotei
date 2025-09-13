namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated enumerable command, and enumerate the
/// records produced by that execution.
/// </summary>
public interface ICommandEnumerator
    : IDisposableEx
    , IEnumerator<IRecord>, IAsyncEnumerator<IRecord>
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
    /// Gets the element in the collection at the current position of the enumerator, or null
    /// if no more ones are available.
    /// </summary>
    new IRecord Current { get; }

    /// <summary>
    /// The schema that describes the records produced by this instance, or <c>null</c> if the
    /// associated command has not been executed yet, or if this information is not available.
    /// </summary>
    ISchema? Schema { get; }
}