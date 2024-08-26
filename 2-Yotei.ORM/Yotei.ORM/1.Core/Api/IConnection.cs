namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database engine.
/// </summary>
[Cloneable]
public partial interface IConnection : IBaseDisposable
{
    /// <summary>
    /// The object that describes the underlying database engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance will try to recover from transient connection errors
    /// before throwing an exception.
    /// </summary>
    int Retries { get; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt to recover from transient
    /// connection errors.
    /// </summary>
    TimeSpan RetryInterval { get; }

    /// <summary>
    /// Gets the transaction associated with this instance.
    /// </summary>
    ITransaction Transaction { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Sets the connectoin associated with the given physical command the the one associated
    /// with this instance.
    /// </summary>
    /// <param name="command"></param>
    void Enlist(IDbCommand command);

    /// <summary>
    /// Determines if this instance is opened or not.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    void Open();

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    ValueTask CloseAsync();
}