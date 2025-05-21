namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underliying relational database.
/// </summary>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    new IXEngine Engine { get; }

    /// <inheritdoc cref="ORM.IConnection.Transaction"/>
    new ITransaction Transaction { get; }

    /// <summary>
    /// The underlying physical connection, if any.
    /// <br/> This property is INFRASTRUCTURE and shall not be used by application code.
    /// </summary>
    DbConnection? DbConnection { get; }
}