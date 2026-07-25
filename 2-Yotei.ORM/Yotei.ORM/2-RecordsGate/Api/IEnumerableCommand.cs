namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented database command that, when executed against its associated
/// connection, enumerates the records produced by that execution.
/// </summary>
[Cloneable]
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
    /// <inheritdoc cref="ICommand.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IEnumerableCommand Clear();
}