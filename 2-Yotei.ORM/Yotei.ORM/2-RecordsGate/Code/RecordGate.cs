namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
public class RecordsGate : IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public RecordsGate(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }
}