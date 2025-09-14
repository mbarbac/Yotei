namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed using its associated connection,
/// enumerates the results produced by that execution.
/// </summary>
/// <remarks>
/// Instances of this type work by wrapping an original enumerable command.
/// </remarks>
/// <typeparam name="T"></typeparam>
public partial interface IEnumerableCommand<T>
    : ICommand
    , IEnumerable<T>, IAsyncEnumerable<T>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    new IEnumerableCommand<T> Clone();

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    new IEnumerableCommand<T> WithConnection(IConnection value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    new IEnumerableCommand<T> WithLocale(Locale value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute this command and enumerate the results produced by that
    /// execution. Returned results may be <c>null</c> if no more ones are available.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator<T> GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate the results produced by that
    /// execution. Returned results may be <c>null</c> if no more ones are available.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator<T> GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// The original command wrapped by this instance.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The delegate to use to convert the original records produced by the execution of this
    /// command to the type of the arbitrary results this instance is created for.
    /// </summary>
    Func<IRecord, T> Converter { get; }
}