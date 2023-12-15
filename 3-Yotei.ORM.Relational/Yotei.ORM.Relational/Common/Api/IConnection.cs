namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underlying relational database.
/// </summary>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    /// </summary>
    new IEngine Engine { get; }

    /// <summary>
    /// The connection string to use when opening this connection, or null if it has not been
    /// set yet. The setter throws an exception is this instance is already connected.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// The server this instance is connected with, or null if this instance is not connected.
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// The database this instance is connected with, or null if this instance is not connected.
    /// </summary>
    string? Database { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The underlying ADO.NET physical connection, or null if this instance is not connected.
    /// </summary>
    internal DbConnection? DbConnection { get; }

    /// <summary>
    /// The default isolation level to use by the transactions associated with this connection.
    /// The setter throws an exception if the default transaction is active.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Transaction"/>
    /// </summary>
    /// <returns></returns>
    new ITransaction Transaction { get; }

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Records"/>
    /// </summary>
    new Records.IRecordsGate Records { get; }
}