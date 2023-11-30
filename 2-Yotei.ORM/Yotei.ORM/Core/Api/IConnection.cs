namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IBaseDisposable
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance will try to recover from transient connection errors.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt to recover from a transient
    /// connectivity error.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this connection is open, or not.
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
    ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    ValueTask CloseAsync();
    
    // ----------------------------------------------------

    /// <summary>
    /// The default transaction used by this instance.
    /// </summary>
    ITransaction Transaction { get; }
}