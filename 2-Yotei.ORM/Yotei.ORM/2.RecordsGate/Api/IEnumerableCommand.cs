namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed using its associated connection,
/// enumerates the records produced by that execution.
/// </summary>
public interface IEnumerableCommand
    : ICommand
    , IEnumerable<IRecord>, IAsyncEnumerable<IRecord>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    new IEnumerableCommand Clone();

    /// <inheritdoc cref="ICommand.WithConnection(IConnection)"/>
    new IEnumerableCommand WithConnection(IConnection value);

    /// <inheritdoc cref="ICommand.WithLocale(Locale)"/>
    new IEnumerableCommand WithLocale(Locale value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEnumerableCommand WithSkip(int value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEnumerableCommand WithTake(int value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by that
    /// execution. Returned records may be <c>null</c> if no more ones are available.
    /// </summary>
    /// <returns></returns>
    new ICommandEnumerator GetEnumerator();

    /// <summary>
    /// Returns an object that can execute this command and enumerate the records produced by that
    /// execution. Returned records may be <c>null</c> if no more ones are available.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance supports native paging based upon its captured contents only.
    /// Later, this value will be combined with the engine's one to determine if native paging is
    /// available or rather shall be emulated by the framework.
    /// </summary>
    bool SupportsNativePaging { get; }

    /// <summary>
    /// The number of records to skip, or cero to ignore this property.
    /// </summary>
    int Skip { get; set; }

    /// <summary>
    /// The number of records to take, or cero to ignore this property.
    /// </summary>
    int Take { get; set; }

    // ----------------------------------------------------

    /// <inheritdoc cref="ICommand.Clear"/>
    new IEnumerableCommand Clear();
}