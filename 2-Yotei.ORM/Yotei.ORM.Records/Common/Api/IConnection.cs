namespace Yotei.ORM.Records;

// ========================================================
/// <inheritdoc cref="ORM.IConnection"/>
[Cloneable]
public partial interface IConnection : ORM.IConnection
{
    /// <inheritdoc cref="ORM.IConnection.Engine"/>
    new IEngine Engine { get; }

    /// <summary>
    /// Provides access to the records-oriented capabilities of this connection.
    /// </summary>
    IRecordMethods Records { get; }
}