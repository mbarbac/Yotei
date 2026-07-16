namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a transaction associated with a given connection.
/// </summary>
public interface ITransaction : IDisposableEx
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to start this transaction.
    /// <br/> This method is INFRASTRUCTURE only, not intended for application usage.
    /// <br/> Inheritors must invoke their base method first.
    /// </summary>
    internal void Start();

    /// <summary>
    /// Invoked to start this transaction.
    /// <br/> This method is INFRASTRUCTURE only, not intended for application usage.
    /// <br/> Inheritors must invoke their base method first.
    /// </summary>
    /// <param name="token"></param>
    internal ValueTask StartAsync(CancellationToken token);

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