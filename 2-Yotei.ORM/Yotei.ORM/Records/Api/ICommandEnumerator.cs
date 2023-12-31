namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute a records-oriented enumerable command and iterate over
/// the set of records produced by that execution.
/// </summary>
public interface ICommandEnumerator : IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token this instance uses for its asynchronous operations.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The schema that describes the structure and contents of the set of records returned by
    /// the execution of the associated command.
    /// </summary>
    ISchema? Schema { get; }
}