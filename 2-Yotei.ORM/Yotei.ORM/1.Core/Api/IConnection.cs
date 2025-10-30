namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// The descriptor of the database engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance tries to recover from a transient connection error
    /// before throwing an exception. If the value of this property is cero, connection is only
    /// tried once.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before the next attempt to recover from a transient
    /// connection error. If it is a zero-alike one, then there will be no wait.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this connection is active, or not.
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