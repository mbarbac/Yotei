namespace Yotei.ORM.Relational.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
public class RecordsGate : ORM.Records.Code.RecordsGate, IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public RecordsGate(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new IConnection Connection => (IConnection)base.Connection;
}