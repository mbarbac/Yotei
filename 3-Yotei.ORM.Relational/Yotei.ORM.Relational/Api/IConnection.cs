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
    /// The underlying ADO.NET connection used by this instance, or <see langword="null"/> if
    /// it is not connected.
    /// <br/> This property is provided for informational purposes only.
    /// </summary>
    DbConnection? DbConnection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ORM.IConnection.Transaction"/>
    /// </summary>
    new ITransaction? Transaction { get; }

    /// <summary>
    /// The isolation level used when starting a new database transaction.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }
}