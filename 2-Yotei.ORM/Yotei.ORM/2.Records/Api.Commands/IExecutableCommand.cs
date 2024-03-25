namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that when executed against its associated connection
/// produces an integer as the result of that execution.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command and return the integer produced by that
    /// execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();
}