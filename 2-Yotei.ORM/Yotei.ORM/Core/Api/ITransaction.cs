namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable transaction associated with a given connection.
/// </summary>
public interface ITransaction : IBaseDisposable
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Determines if this instance is an active one, or not.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// The nesting level of this instance, the number of times it has been started and not yet
    /// committed.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Starts the underlying physical transaction or, if it is already started, increases the
    /// nesting level of this instance.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts the underlying physical transaction or, if it is already started, increases the
    /// nesting level of this instance.
    /// </summary>
    /// <param name="token"></param>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Decreases the nesting level of this instance and, when its nesting level reaches cero,
    /// commits the underlying physical transaction. If it was not started, then an exception is
    /// thrown.
    /// </summary>
    void Commit();

    /// <summary>
    /// Decreases the nesting level of this instance and, when its nesting level reaches cero,
    /// commits the underlying physical transaction. If it was not started, then an exception is
    /// thrown.
    /// </summary>
    /// <param name="token"></param>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if any. Resets the nesting
    /// level of this instance.
    /// </summary>
    void Abort();

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if any. Resets the nesting
    /// level of this instance.
    /// </summary>
    ValueTask AbortAsync();
}