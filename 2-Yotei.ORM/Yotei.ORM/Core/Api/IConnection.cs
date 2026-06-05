namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IAsyncDisposableEx
{
    /// <summary>
    /// The descriptor of the database engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance tries to recover from transient connection errors.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The period of time that this instance waits before attempting to recover from transient
    /// connection errors
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// The period of time this instance waits to obtain an internal lock.
    /// </summary>
    TimeSpan LockInterval { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is active (connected) or not.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// <br/> If the connection was already opened, then this method is ignored.
    /// </summary>
    void Open();

    /// <summary>
    /// Opens the connection with the underlying database.
    /// <br/> If the connection was already opened, then this method is ignored.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database.
    /// <br/> If the connection was already closed, then this method is ignored.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the connection with the underlying database.
    /// <br/> If the connection was already closed, then this method is ignored.
    /// </summary>
    ValueTask CloseAsync();

    // ----------------------------------------------------

    /// <summary>
    /// The database transaction this instance is associated with, or null if any.
    /// </summary>
    ITransaction? Transaction { get; internal set; }

    /// <summary>
    /// Starts the active database transaction associated with this instance.
    /// </summary>
    /// <returns></returns>
    ITransaction StartTransaction();

    /// <summary>
    /// Starts the active database transaction associated with this instance.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default);
}