namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the records produced
/// by that execution.
/// </summary>
public interface ICommandEnumerator
    : IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>, IAsyncDisposableEx
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token assigned to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The record at the current position of this enumerator, or <see langword="null"/> if it
    /// has not been yet executed, or if there are no more results available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// Determines if the schema shall be captured, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ICommandEnumerator WithSchema(bool value);

    /// <summary>
    /// Obtains a value that indicates if, when executing the command, the schema will be captured
    /// or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    void WithSchema(out bool value);
}