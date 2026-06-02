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
}