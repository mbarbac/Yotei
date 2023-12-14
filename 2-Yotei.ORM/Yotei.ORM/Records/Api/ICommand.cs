namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that can be executed against an underlying database.
/// </summary>
public interface ICommand : ORM.ICommand
{
    /// <summary>
    /// <inheritdoc cref="ORM.ICommand.ExecuteReader"/>
    /// </summary>
    /// <returns></returns>
    new ICommandReader ExecuteReader();

    /// <summary>
    /// <inheritdoc cref="ORM.ICommand.ExecuteReaderAsync(CancellationToken)"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    new ValueTask<ICommandReader> ExecuteReaderAsync(CancellationToken token = default);
}