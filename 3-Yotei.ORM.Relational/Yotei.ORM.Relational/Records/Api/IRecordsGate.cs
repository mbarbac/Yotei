namespace Yotei.ORM.Relational.Records;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.Records.IRecordsGate"/>
/// </summary>
public interface IRecordsGate : ORM.Records.IRecordsGate
{
    /// <summary>
    /// <inheritdoc cref="ORM.Records.IRecordsGate.Connection"/>
    /// </summary>
    new IConnection Connection { get; }
}