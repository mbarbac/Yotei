namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underliying database.
/// </summary>
[Cloneable]
public partial interface IConnection : IBaseDisposable
{
    /// <summary>
    /// Describes the underlying engine this instance connects to.
    /// </summary>
    IEngine Engine { get; }
}