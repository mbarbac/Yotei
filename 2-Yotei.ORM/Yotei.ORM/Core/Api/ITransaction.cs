namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a transaction associated with a given connection.
/// </summary>
public interface ITransaction : IDisposableEx
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }
}