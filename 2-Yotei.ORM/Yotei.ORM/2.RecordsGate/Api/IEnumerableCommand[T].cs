namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed using its associated connection,
/// enumerates the arbitrary results produced by that execution, using the given delegate to
/// convert from the original records to the type of those results.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
[InheritWiths]
public partial interface IEnumerableCommand<T>
    : ICommand
    , IEnumerable<T?>, IAsyncEnumerable<T?>
{
    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by that
    /// execution, if any.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator<T> GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by that
    /// execution, if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator<T> GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// The original command this instance wraps.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The delegate to use to convert the original records produced by the execution of this
    /// command to the type of the arbitrary results this instance is created for.
    /// </summary>
    Func<IRecord, ISchema?, T> Converter { get; }
}