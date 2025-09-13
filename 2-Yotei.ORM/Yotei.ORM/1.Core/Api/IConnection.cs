namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
public partial interface IConnection : IDisposableEx
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IConnection Clone();

    // ----------------------------------------------------

    /// <summary>
    /// Describes the underlying engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance will try to recover from transient connection errors
    /// before throwing an exception. The value of this property is positive and greater than
    /// cero.
    /// </summary>
    int Retries { get; }

    /// <summary>
    /// The amount of time this instance waits before a new attempt to recover from transient
    /// connection errors. The value of this property must conceptually be a positive number,
    /// or cero if no wait time is requested.
    /// </summary>
    TimeSpan RetryInterval { get; }

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

    // ----------------------------------------------------

    /// <summary>
    /// The default transaction associated with this instance.
    /// <br/> This object is automatically disposed along with this connection, client code must
    /// not try to manage its life cycle.
    /// </summary>
    ITransaction Transaction { get; }

    /// <summary>
    /// The collection of value converters from application-level types to database-level ones.
    /// </summary>
    IValueConverterList ValueConverters { get; }

    /// <summary>
    /// Provides access to the records-oriented capabilities of this instance.
    /// </summary>
    IRecordsGate Records { get; }
}