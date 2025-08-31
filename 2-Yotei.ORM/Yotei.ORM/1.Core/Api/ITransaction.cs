namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable database transaction associated with a given connection.
/// <br/> For simplicity of the nestable API, instances of this type ARE NOT STARTED when created,
/// but rather when such is explicitly requested, either starting the underlying physical one or
/// increasing their nesting level. Similarly, committing either decreases the nesting level or,
/// if needed, commits the underlying physical transaction. Finally, aborting inconditionally
/// aborts the underlying physical transaction despite the nesting level.
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