namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underliying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IDisposableEx
{
    /// <summary>
    /// Describes the underlying engine this instance connects to.
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
    /// Gets the default transaction associated with this instance.
    /// </summary>
    ITransaction Transaction { get; }

    // ----------------------------------------------------

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