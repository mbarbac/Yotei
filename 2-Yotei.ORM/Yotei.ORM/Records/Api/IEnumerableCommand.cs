namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that can enumerate the records produced by its execution.
/// </summary>
public partial interface IEnumerableCommand
    : ICommand
    , IEnumerable<IRecord?>
    , IAsyncEnumerable<IRecord?>
{
    /// <summary>
    /// Determines if this instance support native paging, or rather it has to be emulated by
    /// the framework.
    /// </summary>
    bool NativePaging { get; }

    /// <summary>
    /// The number of records to skip, or a negative value if this property is ignored.
    /// </summary>
    [WithGenerator]
    int Skip { get; set; }

    /// <summary>
    /// The number of records to take, or a negative value if this property is ignored.
    /// </summary>
    [WithGenerator]
    int Take { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute this command and enumerate through the records produced
    /// by that execution.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate through the records produced
    /// by that execution.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);
}