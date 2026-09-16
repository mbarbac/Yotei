namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a transaction associated with a given connection.
/// </summary>
public interface ITransaction : IAsyncDisposableEx
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Deermines if this instance is an active one, or not.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Starts this transaction.
    /// <br/> Sets it as the current one in its associated connection, if possible.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts this transaction.
    /// <br/> Sets it as the current one in its associated connection, if possible.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask StartAsync(CancellationToken token = default);

    /// <summary>
    /// Commits this transaction.
    /// </summary>
    void Commit();

    /// <summary>
    /// Commits this transaction.
    /// </summary>
    /// <param name="token"></param>
    ValueTask CommitAsync(CancellationToken token = default);

    /// <summary>
    /// Aborts this transaction.
    /// </summary>
    void Abort();

    /// <summary>
    /// Aborts this transaction.
    /// </summary>
    ValueTask AbortAsync();
}