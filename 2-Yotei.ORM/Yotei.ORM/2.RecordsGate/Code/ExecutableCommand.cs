namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IExecutableCommand"/>
[Cloneable<IExecutableCommand>]
[InheritWiths<IExecutableCommand>]
public abstract partial class ExecutableCommand : Command, IExecutableCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public ExecutableCommand(IConnection connection) : base(connection) { }

    // ----------------------------------------------------

    public virtual ICommandExecutor GetExecutor()
    {
        var temp = Connection.Records.CreateCommandExecutor(this);
        return temp;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract override IExecutableCommand Clear();
}