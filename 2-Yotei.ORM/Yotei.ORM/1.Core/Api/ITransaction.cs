namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable database transaction associated with a given connection.
/// <br/> Note that instances of this type ARE NOT STARTED automatically when created, but rather
/// it is done manually when needed.
/// </summary>
public interface ITransaction : IDisposableEx
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

    /// <summary>
    /// Determines if this instance has opened the associated connection, or not.
    /// </summary>
    bool HasOpenedConnection { get; }

    // ----------------------------------------------------

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