namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute a record-oriented enumerable command and enumerate the
/// records that execution produces.
/// </summary>
public interface ICommandEnumerator
    : IEnumerator<IRecord>, IAsyncEnumerator<IRecord>, IBaseDisposable
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token given to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The schema that describes the structure and contents of the records produced by the
    /// execution of the associated command. The value of this property is null if the command
    /// has not been executed yet.
    /// </summary>
    ISchema? Schema { get; }

    /// <summary>
    /// Gets the result produced by the current iteration of this instance, or null if no more
    /// results are available, or if the command has not been executed yet.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to
    /// obtain the next available result. Returns true is there is an available result, or false
    /// otherwise.
    /// </summary>
    /// <returns></returns>
    new bool MoveNext();

    /// <summary>
    /// Executes the associated command, if it has not been executed yet, and then tries to
    /// obtain the next available result. Returns true is there is an available result, or false
    /// otherwise.
    /// </summary>
    /// <returns></returns>
    new ValueTask<bool> MoveNextAsync();
}