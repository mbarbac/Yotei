namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underliying relational database.
/// </summary>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    new IEngine Engine { get; }

    /// <inheritdoc cref="ORM.IConnection.Records"/>
    new IRecordsGate Records { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The default isolation level of transactions associated with this instance.
    /// <br/> The setter throws an exception if there is an active existing transaction.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// The connection string used by this instance, or <c>null</c> if its value has not been
    /// set yet. The setter throws an exception if the connection is opened.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// The server this instance connects to, or <c>null</c> if this information is not available.
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// The database, or catalog. this instance connects to, or <c>null</c> if this information
    /// is not available.
    /// </summary>
    string? Database { get; }

    /// <inheritdoc cref="ORM.IConnection.Transaction"/>
    new ITransaction Transaction { get; }

    /// <summary>
    /// The underlying physical connection, if any.
    /// <br/> This property is INFRASTRUCTURE and shall NOT be used by application code.
    /// </summary>
    DbConnection? DbConnection { get; }
}