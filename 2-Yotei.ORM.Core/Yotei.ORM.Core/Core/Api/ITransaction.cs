namespace Yotei.ORM.Core;

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
    /// Determines if this instance is active, or not.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// The current nesting level of this transaction. A value of cero means this transaction has
    /// not yet been started.
    /// </summary>
    int Level { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Either starts the underlying physical transaction, if this instance is not yet started,
    /// or increases its nesting level.
    /// </summary>
    void Start();

    /// <summary>
    /// Either starts the underlying physical transaction, if this instance is not yet started,
    /// or increases its nesting level.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Either commits the underlying physical transaction, if this instance is not nested, or
    /// decreases its nesting level. If this instance was not started, an exception is thrown.
    /// </summary>
    void Commit();

    /// <summary>
    /// Either commits the underlying physical transaction, if this instance is not nested, or
    /// decreases its nesting level. If this instance was not started, an exception is thrown.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction and resets the nesting level
    /// of this instance.
    /// </summary>
    void Abort();

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction and resets the nesting level
    /// of this instance.
    /// </summary>
    /// <returns></returns>
    ValueTask AbortAsync();
}