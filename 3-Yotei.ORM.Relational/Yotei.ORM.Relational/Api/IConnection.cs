namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underlying ADO.NET-based relational database.
/// </summary>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    /// </summary>
    new IEngine Engine { get; }

    /// <summary>
    /// The connection string used by this instance, or <see langword="null"/> if its value has
    /// not been set yet. The setter throws an exception if this instance is active.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// The server this instance connects to, or <see langword="null"/> if this information is
    /// not available.
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// The database this instance connects to, or <see langword="null"/> if this information is
    /// not available.
    /// </summary>
    string? Database { get; }

    /// <summary>
    /// The underlying ADO.NET connection used by this instance, or <see langword="null"/> if
    /// it is not connected.
    /// <br/> This property is provided for informational purposes only.
    /// </summary>
    DbConnection? DbConnection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The isolation level used when starting a new database transaction.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Transaction"/>
    /// </summary>
    new ITransaction? Transaction { get; }

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.StartTransaction"/>
    /// </summary>
    /// <returns></returns>
    new ITransaction StartTransaction();

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.StartTransaction"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default);
}