namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that can enumerate the records produced by its execution.
/// </summary>
public interface IEnumerableCommand
    : ICommand
    , IEnumerable<IRecord?>
    , IAsyncEnumerable<IRecord?>
{
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