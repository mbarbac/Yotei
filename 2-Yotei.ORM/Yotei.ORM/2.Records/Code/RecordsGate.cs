namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
public abstract class RecordsGate : IRecordsGate
{
    /// <summary>
    /// Initializes a new intance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <inheritdoc/>
    public IConnection Connection { get; }
}