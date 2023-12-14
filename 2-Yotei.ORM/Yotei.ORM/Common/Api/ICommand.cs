namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary command that can be executed against an underlying database.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database,
    /// using the default iterable mode of this command.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database,
    /// either for an iterable environment or not.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);

    /// <summary>
    /// Executes this command and returns an enumeration of the results that execution produces.
    /// </summary>
    /// <returns></returns>
    IReader ExecuteReader();

    /// <summary>
    /// Executes this command and returns an enumeration of the results that execution produces.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<IReader> ExecuteReaderAsync(CancellationToken token = default);

    /// <summary>
    /// Executes this command and returns an integer as the result of that operation.
    /// </summary>
    /// <returns></returns>
    int ExecuteScalar();

    /// <summary>
    /// Executes this command and returns an integer as the result of that operation.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<int> ExecuteScalarAsync(CancellationToken token = default);
}