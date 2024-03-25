namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that when executed against its associated connection
/// enumerates the records produced by that execution, if any.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IEnumerableCommand
    : ICommand
    , IEnumerable<IRecord?>, IAsyncEnumerable<IRecord?>
{
    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by
    /// that execution.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by
    /// that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance supports native paging capabilities, or rather they have to
    /// be emulated by the framework.
    /// </summary>
    bool NativePaging { get; }

    /// <summary>
    /// The number of records to skip, or a negative value to ignore this setting.
    /// </summary>
    [WithGenerator]
    int Skip { get; set; }

    /// <summary>
    /// The number of records to take, or a negative value to ignore this setting.
    /// </summary>
    [WithGenerator]
    int Take { get; set; }
}