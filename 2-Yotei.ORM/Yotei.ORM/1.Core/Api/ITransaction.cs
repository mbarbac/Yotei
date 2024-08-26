namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable database transaction.
/// </summary>
public interface ITransaction : IBaseDisposable
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Determines if this transaction is active, or not.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets the current nesting level of this instance. A value of zero means that it has not
    /// been started yet.
    /// </summary>
    int Level { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Sets the transaction associated with the given physical command the the one associated
    /// with this instance. In addition, does the same with the associated connection if deemed
    /// neccesary.
    /// </summary>
    /// <param name="command"></param>
    void Enlist(IDbCommand command);

    /// <summary>
    /// Starts the underlying physical transaction, if it has not been started yet, or otherwise
    /// just increases its nesting level.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts the underlying physical transaction, if it has not been started yet, or otherwise
    /// just increases its nesting level.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Commits the underlying physical transaction, if this instance has been started just once,
    /// or decreases its nesting level otherwise.
    /// </summary>
    void Commit();

    /// <summary>
    /// Commits the underlying physical transaction, if this instance has been started just once,
    /// or decreases its nesting level otherwise.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if it has been started, and
    /// then resets the nesting level to zero.
    /// </summary>
    void Abort();

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if it has been started, and
    /// then resets the nesting level to zero.
    /// </summary>
    /// <returns></returns>
    ValueTask AbortAsync();
}