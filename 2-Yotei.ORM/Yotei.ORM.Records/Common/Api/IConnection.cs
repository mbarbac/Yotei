namespace Yotei.ORM.Records;

// ========================================================
/// <inheritdoc cref="ORM.IConnection"/>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    new IEngine Engine { get; }
}