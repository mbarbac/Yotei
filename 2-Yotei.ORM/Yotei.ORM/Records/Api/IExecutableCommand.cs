namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed, returns an integer as the result
/// of that execution.
/// </summary>
public interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Obtains an executor for this command.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();
}