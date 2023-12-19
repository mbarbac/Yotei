
namespace Yotei.ORM.Relational.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.Records.ICommandExecutor"/>
/// </summary>
public class CommandExecutor : ORM.Records.Code.CommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public CommandExecutor(ORM.Records.ICommand command) : base(command) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override int OnExecute() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override ValueTask<int> OnExecuteAsync(CancellationToken token) => throw null;
}