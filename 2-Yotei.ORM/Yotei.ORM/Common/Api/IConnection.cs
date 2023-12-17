namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IBaseDisposable
{
    /// <summary>
    /// The object that describes the underlying database engine used by this instance.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance will retry to recover from transient network errors.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt to recover from a transient
    /// network error.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this connection is open or not.
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
    ValueTask CloseAsync();
}