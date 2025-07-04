﻿namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
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
    /// The collection of value converters from application-level values to database-level ones
    /// maintained by this instance.
    /// </summary>
    IValueConverterList ToDatabaseConverters { get; }

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

    // ----------------------------------------------------
    
    /// <summary>
    /// Invoked to create a new transaction of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    ITransaction CreateTransaction();

    /// <summary>
    /// Provides access to the records-oriented capabilities of this instance.
    /// </summary>
    IRecordsGate Records { get; }
}