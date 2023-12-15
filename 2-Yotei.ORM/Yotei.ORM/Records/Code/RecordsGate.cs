namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecordsGate"/>
/// </summary>
[SuppressMessage("", "IDE0290")]
public class RecordsGate : IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRawCommand Raw() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IQueryCommand Query() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public IInsertCommand Insert(Func<dynamic, object> table) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public IUpdateCommand Update(Func<dynamic, object> table) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public IDeleteCommand Delete(Func<dynamic, object> table) => throw null;
}