namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IExecutableCommand"/>
/// </summary>
public abstract class ExecutableCommand : ORM.Code.Command, IExecutableCommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public ExecutableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandExecutor GetExecutor() => Connection.Records.CommandExecutor(this);
}