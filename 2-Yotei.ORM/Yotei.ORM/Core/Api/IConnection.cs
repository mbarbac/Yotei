namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IAsyncDisposableEx
{
    /// <summary>
    /// The kind of database engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The number of times this instance tries to recover from transient connection errors before
    /// throwing an exception.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before the next attemp of recovering from transient
    /// connection errors.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// Determines if this connection is active or not.
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
    /// If not <see langword="null"/>, then the current active transaction associated with this
    /// instance.
    /// </summary>
    ITransaction? Transaction { get; }

    /// <summary>
    /// Creates and starts a default transaction associated to this instance, that becomes its
    /// current one.
    /// <br/> This method throws an exception if there is already an active transaction.
    /// </summary>
    /// <returns></returns>
    ITransaction StartTransaction();

    /// <summary>
    /// Creates and starts a default transaction associated to this instance, that becomes its
    /// current one. This method throws an exception if there is already an active transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default);

    /// <summary>
    /// Invoked by the given transaction when it has been committed or aborted.
    /// </summary>
    /// <param name="transaction"></param>
    internal void EndTransaction(ITransaction transaction);

    /// <summary>
    /// Invoked by the given transaction when it has been committed or aborted.
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal ValueTask EndTransactionAsync(ITransaction transaction, CancellationToken token = default);
}