namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IExecutableCommand"/>
public abstract class ExecutableCommand : Command, IExecutableCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public ExecutableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ExecutableCommand(ExecutableCommand source) : base(source) { }

    /// <inheritdoc/>
    public abstract override IExecutableCommand Clone();

    /// <inheritdoc/>
    public abstract override IExecutableCommand WithConnection(IConnection value);

    /// <inheritdoc/>
    public abstract override IExecutableCommand WithLocale(Locale value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract override IExecutableCommand Clear();
}