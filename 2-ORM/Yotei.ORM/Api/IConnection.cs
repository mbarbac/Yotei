namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection context with an underlying database.
/// </summary>
public interface IConnection : ICloneable, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns><inheritdoc cref="ICloneable.Clone"/></returns>
    new IConnection Clone();

    /// <summary>
    /// The engine used by this connection.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance tries to recover from transient connectivity error
    /// conditions.
    /// </summary>
    uint Retries { get; set; }

    /// <summary>
    /// The interval of time this instance waits before trying to recover from transient
    /// connectivity conditions.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// Whether this instance is connected with its underlying database, or not.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database. This method just returns if this
    /// connection was already opened.
    /// </summary>
    void Open();

    /// <summary>
    /// Opens the connection with the underlying database. This method just returns if this
    /// connection was already opened.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database. This method just returns if this
    /// connection was already closed.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the connection with the underlying database. This method just returns if this
    /// connection was already closed.
    /// </summary>
    /// <returns></returns>
    ValueTask CloseAsync();
}