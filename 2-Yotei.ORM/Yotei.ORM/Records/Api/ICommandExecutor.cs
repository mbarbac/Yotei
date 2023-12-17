namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ICommandExecutor"/>
/// </summary>
public interface ICommandExecutor : ORM.ICommandExecutor
{
    /// <summary>
    /// <inheritdoc cref="ICommandExecutor.ICommand"/>
    /// </summary>
    new ICommand Command { get; }
}