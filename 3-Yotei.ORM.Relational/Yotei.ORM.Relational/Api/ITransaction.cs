using System.Data;

namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a nestable database transaction associated with a given relational connection.
/// </summary>
public interface ITransaction : ORM.ITransaction
{
    /// <inheritdoc cref="ORM.ITransaction.Connection"/>
    new IConnection Connection { get; }

    /// <summary>
    /// The isolation level of this instance.
    /// <br/> The setter throws an exception if this instance is active.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// The underlying physical transaction, if any.
    /// <br/> This property is INFRASTRUCTURE and shall not be used by application code.
    /// </summary>
    DbTransaction? DbTransaction { get; }
}