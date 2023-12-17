namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command and enumerate through the
/// results produced by that execution.
/// </summary>
public interface ICommandEnumerator : IEnumerator, IAsyncEnumerator<object>, IBaseDisposable
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    ICommand Command { get; }
}