namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the results produced
/// by that execution.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandEnumerator<T> : IEnumerator<T?>, IAsyncEnumerator<T?>, IDisposableEx
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token passed to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The result at the current position of this enumerator, or <see langword="null"/> if it
    /// has not been yet executed, or if there are no more results available.
    /// </summary>
    new T? Current { get; }

    /// <summary>
    /// The delegate that converts from the underlying records produced by the command, to the
    /// desired results of the enumerator.
    /// <br/> Nota that <see cref="IRecord"/> instances can be used as <see langword="dynamic"/>
    /// objects to facilitate the conversions.
    /// </summary>
    Func<IRecord, T> Converter { get; }
}