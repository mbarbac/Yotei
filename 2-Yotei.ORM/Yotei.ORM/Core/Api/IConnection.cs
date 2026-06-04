namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IAsyncDisposableEx
{
    /// <summary>
    /// The descriptor of the database engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }
}