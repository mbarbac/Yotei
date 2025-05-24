namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed, enumerates the records produced
/// by that execution, if any.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IEnumerableCommand
    : ICommand
    , IEnumerable<IRecord?>, IAsyncEnumerable<IRecord?>
{
    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by
    /// that execution, if any.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by
    /// that execution, if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance supports native paging, based upon both its current contents
    /// and the capabilities of the underlying engine, or rather if they have to be emulated by
    /// the framework.
    /// </summary>
    bool SupportsNativePaging { get; }

    /// <summary>
    /// The number of records to skip, or a negative value to ignore this property.
    /// </summary>
    [With] int Skip { get; set; }

    /// <summary>
    /// The number of records to take, or a negative value to ignore this property.
    /// </summary>
    [With] int Take { get; set; }
}