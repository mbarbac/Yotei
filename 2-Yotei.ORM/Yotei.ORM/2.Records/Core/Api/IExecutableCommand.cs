namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that, when executed, produces an integer as the result
/// of that execution.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IExecutableCommand : ICommand
{
    /// <summary>
    /// Returns an object that can execute this command and produce the integer that results from
    /// that execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

    // ----------------------------------------------------

    /// <inheritdoc cref="ICommand.Clear"/>
    new IExecutableCommand Clear();
}