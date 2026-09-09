namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the strongly typed
/// results produced by that execution, after appropriately converted.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandEnumerator<T>
    : IEnumerator<T?>, IAsyncEnumerator<T?>, IAsyncDisposableEx
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token assigned to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The delegate to invoke to converte the dynamic records returned by the execution of the
    /// command into the strongly typed results to be returned by this instance.
    /// </summary>
    Func<dynamic, T> Converter { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The result at the current position of this enumerator, or <see langword="null"/> if it
    /// has not been yet executed, or if there are no more results available.
    /// <br/> The actual value of this property may be built each time its getter is called, so
    /// proper caching is advised if needed.
    /// </summary>
    new T? Current { get; }

    /// <summary>
    /// Determines if the schema shall be captured, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ICommandEnumerator<T> WithSchema(bool value);

    /// <summary>
    /// Obtains a value that indicates if, when executing the command, the schema will be captured
    /// or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    void WithSchema(out bool value);
}