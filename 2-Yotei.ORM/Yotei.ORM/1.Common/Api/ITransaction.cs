namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable transaction under which a set of commands can be executed as a unit of
/// work against an underlying database.
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
    /// The nesting level of this instance. A value of cero means this transaction has not been
    /// started yet.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Starts the underlying physical transaction, if it has not been started yet, and increases
    /// its nesting level.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts the underlying physical transaction, if it has not been started yet, and increases
    /// its nesting level.
    /// </summary>
    /// <param name="token"></param>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Commits the underlying physical transaction, or decreases its nesting level.
    /// </summary>
    void Commit();

    /// <summary>
    /// Commits the underlying physical transaction, or decreases its nesting level.
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