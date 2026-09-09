namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command that, when executed against its associated connection, returns
/// an integer as the ersult of that execution.
/// </summary>
[Cloneable]
public partial interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command and return an integer as the result of
    /// that execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICommand.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IExecutableCommand Clear();
}