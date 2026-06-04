namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a transaction associated with a given ADO.NET-based relational connection.
/// </summary>
public interface ITransaction : ORM.ITransaction
{
    /// <summary>
    /// <inheritdoc cref="ORM.ITransaction.Connection"/>
    /// </summary>
    new IConnection Connection { get; }

    /// <summary>
    /// The underlying ADO.NET transaction used by this instance.
    /// <br/> This property is provided for informational purposes only.
    /// </summary>
    DbTransaction DbTransaction { get; }

    /// <summary>
    /// The transaction locking behavior used by this instance.
    /// </summary>
    IsolationLevel IsolationLevel { get; }
}