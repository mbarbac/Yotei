namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
/// <param name="connection"
public abstract class RecordsGate(IConnection connection) : IRecordsGate
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; } = connection.ThrowWhenNull();
}