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

    /// <summary>
    /// Determines the connection that has attached this instance to it.
    /// </summary>
    internal IConnection? Attached { get; set; }

    /// <summary>
    /// Determines if obtaining this instance has opened the associated connection.
    /// </summary>
    internal bool HasOpenedConnection { get; set; }

    // ----------------------------------------------------

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