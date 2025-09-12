namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated enumerable command, and enumerate the
/// results produced by that execution, using a given delegate to convert from the original
/// records produced by that execution to the type of those results.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandEnumerator<T>
    : IDisposableEx
    , IEnumerator<T?>, IAsyncEnumerator<T?>
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand<T> Command { get; }

    /// <summary>
    /// The cancellation token used by this instance.
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Gets the record produced by the current iteration of the execution of the command, or
    /// <c>null</c> if it has not been executed yet, or if there are no more results available.
    /// </summary>
    new T? Current { get; }

    /// <summary>
    /// The schema that describes the records produced by this instance, or <c>null</c> if the
    /// associated command has not been executed yet, or if this information is not available.
    /// </summary>
    ISchema? Schema { get; }
}