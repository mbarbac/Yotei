namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underlying ADO.NET database.
/// </summary>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    /// </summary>
    new IEngine Engine { get; }

    /// <summary>
    /// The connection string used by this instance to open the underlying connection, or null
    /// if its value has not been set. The setter throws an exception if the connection is in
    /// use.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// The server this instance connects to, or null if this information is not available.
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// The database this instance connects to, or null if this information is not available.
    /// </summary>
    string? Database { get; }

    /// <summary>
    /// The underlying physical connection.
    /// </summary>
    DbConnection? DbConnection { get; }
}