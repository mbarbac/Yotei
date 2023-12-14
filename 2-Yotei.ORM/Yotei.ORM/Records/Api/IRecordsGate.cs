namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the record-oriented capabilities of a given connection.
/// </summary>
public interface IRecordsGate
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Returns a new raw command, whose contents can be set explicitly.
    /// </summary>
    /// <returns></returns>
    IRawCommand Raw();

    /// <summary>
    /// Returns a new query command.
    /// </summary>
    /// <returns></returns>
    IQueryCommand Query();

    /// <summary>
    /// Returns a new insert command for the given primary table.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    IInsertCommand Insert(Func<dynamic, object> table);

    /// <summary>
    /// Returns a new update command for the given primary table.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    IUpdateCommand Update(Func<dynamic, object> table);

    /// <summary>
    /// Returns a new delete command for the given primary table.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    IDeleteCommand Delete(Func<dynamic, object> table);
}