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
    /// The number of times this instance will retry to recover from transient errors.
    /// </summary>
    int Retries { get; set; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt to recover from a transient
    /// error.
    /// </summary>
    TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// The default locale to use with culture-sensitive elements in the underlying database.
    /// <br/> The default value of this property is obtained from the thread from which it was
    /// created, or from the instance from which this one was cloned.
    /// </summary>
    Locale Locale { get; set; }

    /// <summary>
    /// The collection of value translators used by this instance to translate application-level
    /// values to the ones understood by the underlying database.
    /// </summary>
    IValueTranslators ValueTranslators { get; }

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

    // ----------------------------------------------------

    /// <summary>
    /// The default nestable transaction associated with this instance.
    /// </summary>
    ITransaction Transaction { get; }
}