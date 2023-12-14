namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a transaction under which a set of commands are executed against an underlying
/// database.
/// </summary>
public interface ITransaction : IBaseDisposable
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Determines if this transaction is active or not.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Starts the underlying physical transaction.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Commits the underlying physical transaction.
    /// </summary>
    void Commit();

    /// <summary>
    /// Commits the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction.
    /// </summary>
    void Abort();

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction.
    /// </summary>
    ValueTask AbortAsync();
}