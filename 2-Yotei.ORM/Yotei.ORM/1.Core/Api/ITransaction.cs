namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable transaction associated with a given connection.
/// <br/> Nestable transactions are not started automatically when created. They are logical ones
/// that can be started and committed many times, reverting to the physical transaction only when
/// needed. Aborting a nestable transaction aborts the whole logical chain.
/// </summary>
public interface ITransaction : IAsyncDisposableEx
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Determines if this transaction is active, or not.
    /// </summary>
    bool IsActive { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Either starts the underlying physical transaction or increases its nesting level.
    /// </summary>
    void Start();

    /// <summary>
    /// Either starts the underlying physical transaction or increases its nesting level.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Either commits the underlying physical transaction or decreases its nesting level.
    /// <br/> If this instance was not an active one, an exception is thrown.
    /// </summary>
    void Commit();

    /// <summary>
    /// Either commits the underlying physical transaction or decreases its nesting level.
    /// <br/> If this instance was not an active one, an exception is thrown.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if any, and resets this
    /// instance to a not-active state.
    /// </summary>
    void Abort();

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if any, and resets this
    /// instance to a not-active state.
    /// </summary>
    /// <returns></returns>
    ValueTask AbortAsync();
}