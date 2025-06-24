namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated enumerable command and enumerate the
/// typed results produced by that execution, if any.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandEnumerator<T>
    : IDisposableEx
    , IEnumerator<T?>, IAsyncEnumerator<T?>
    , IEnumerable<T?>, IAsyncEnumerable<T?>
{
    /// <summary>
    /// The records-oriented enumerator this instance is built for.
    /// </summary>
    ICommandEnumerator Enumerator { get; }

    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token used by this instance.
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the result produced by the current iteration of the execution of the associated
    /// command, or <c>null</c> if it has not been executed yet, or if there are no more results
    /// available.
    /// </summary>
    new T? Current { get; }

    /// <summary>
    /// The delegate to invoke to convert a <see cref="IRecord"/> object to an instance of the
    /// type returned by this iterator.
    /// </summary>
    Func<IRecord, T> Converter { get; }
}