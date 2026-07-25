namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the records produced
/// by that execution.
/// </summary>
public interface ICommandEnumerator
    : IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>
    , IDisposableEx
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The element at the current position of this enumerator, or <see langword="null"/> if it
    /// has not been yet executed, or if there are no more records available.
    /// </summary>
    new IRecord? Current { get; }
}