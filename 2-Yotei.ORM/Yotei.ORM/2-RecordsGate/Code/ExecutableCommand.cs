namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IExecutableCommand"/>
/// </summary>
[Cloneable(ReturnType = typeof(IExecutableCommand))]
public abstract partial class ExecutableCommand : Command, IExecutableCommand
{
    /// <summary>
    /// Initilizes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public ExecutableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected ExecutableCommand(ExecutableCommand other) : base(other) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandExecutor GetExecutor() => Connection.Records.CreateExecutor(this);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IExecutableCommand Clear() => this;
}