namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that returns an integer as the result of its execution.
/// </summary>
public interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command and return the integer produced by that
    /// execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();
}