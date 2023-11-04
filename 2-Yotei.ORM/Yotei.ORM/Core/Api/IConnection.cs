namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IBaseDisposable, ICloneable
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance tries to recover from a transient connection error.
    /// </summary>
    [WithGenerator]
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt of recover from a transient
    /// connectivity error.
    /// </summary>
    [WithGenerator]
    TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// The default locale used with culture-sensitive elements in the database.
    /// </summary>
    [WithGenerator]
    Locale Locale { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this connection is already opened, or not.
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

    // ----------------------------------------------------

    /// <summary>
    /// The default transaction used by this instance.
    /// </summary>
    ITransaction Transaction { get; }
}