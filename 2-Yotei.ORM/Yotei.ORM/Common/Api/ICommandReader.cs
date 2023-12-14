namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a forward-only reader of the data produced by an enumerable command.
/// </summary>
public interface ICommandReader : IEnumerable, IAsyncEnumerable<object>, IBaseDisposable
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The cancellation token to used with async operations.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The text of the command executed against the underlying database.
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// The ordered collection of parameters used by the command when executed.
    /// </summary>
    IParameterList Parameters { get; }
}