namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented database command that, when executed against its associated
/// connection, enumerates the results produced by that execution (which may be records or any
/// other arbitrary result).
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
    /// Adds to the SELECT clause of this command the column specification obtained from parsing
    /// the given dynamic lambda expresion.
    /// <br/> 'x => x.Source.All().As(...)'
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IEnumerableCommand Select(Func<dynamic, object> spec);

    /// <summary>
    /// Specifies the proyection to use to obtain the results of the enumeration.
    /// <br/> 'x => new { Name = x.Name }'
    /// <br/> 'x => new Entity(...)'
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IEnumerableCommand Select<T>(Func<dynamic, T> spec);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this command supports native paging based upon its current captured contents
    /// only. Later, this value is combined with the engine's one to determine is native paging is
    /// available, or rather if it shall be emulated by the framework.
    /// </summary>
    bool SupportsNativePaging { get; }

    /// <summary>
    /// Sets the number of records to skip, or -1 to ignore this setting.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ICommand Skip(int value);

    /// <summary>
    /// Obtains the current value of this setting.
    /// </summary>
    /// <param name="value"></param>
    void Skip(out int value);

    /// <summary>
    /// Sets the number of records to take, or -1 to ignore this setting.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ICommand Take(int value);

    /// <summary>
    /// Obtains the current value of this setting.
    /// </summary>
    /// <param name="value"></param>
    void Take(out int value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICommand.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IEnumerableCommand Clear();
}