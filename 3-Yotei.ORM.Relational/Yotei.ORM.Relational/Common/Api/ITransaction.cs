namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ITransaction"/>
/// </summary>
public interface ITransaction : ORM.ITransaction
{
    /// <summary>
    /// <inheritdoc cref="ORM.ITransaction.Connection"/>
    /// </summary>
    new IConnection Connection { get; }

    /// <summary>
    /// The isolation level of this instance. The setter throws an exception is this instance
    /// is active.
    /// </summary>
    IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// The underlying physical transaction.
    /// </summary>
    internal DbTransaction? DbTransaction { get; }
}