namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
public class RawCommand : EnumerableCommand, IRawCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection)
    {
        throw null;
    }

    /// <summary>
    /// Initializes a new instance with the contents obtained from both parsing the given dynamic
    /// lambda expression, and the optional collection of values for the command arguments (which
    /// are used only when the expression resolves into a string).
    /// <br/> If any values are used, then they must be encoded in the given text using either a
    /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may or
    /// may not start with the engine's prefix. Unused values or dangling specifications are not
    /// allowed.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    /// <param name="range"></param>
    public RawCommand(
        IConnection connection,
        Func<dynamic, object> spec, params object?[]? range) : this(connection) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source)
    {
        throw null;
    }

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    /// <inheritdoc/>
    public override IRawCommand Clone() => new RawCommand(this);
    IEnumerableCommand IEnumerableCommand.Clone() => Clone();
    IExecutableCommand IExecutableCommand.Clone() => Clone();

    /// <inheritdoc/>
    public override IRawCommand WithConnection(IConnection value) => new RawCommand(this) { Connection = value };
    IEnumerableCommand IEnumerableCommand.WithConnection(IConnection value) => WithConnection(value);
    IExecutableCommand IExecutableCommand.WithConnection(IConnection value) => WithConnection(value);

    /// <inheritdoc/>
    public override IRawCommand WithLocale(Locale value) => new RawCommand(this) { Locale = value };
    IEnumerableCommand IEnumerableCommand.WithLocale(Locale value) => WithLocale(value);
    IExecutableCommand IExecutableCommand.WithLocale(Locale value) => WithLocale(value);

    /// <inheritdoc/>
    public override IRawCommand WithSkip(int value) => new RawCommand(this) { Skip = value };

    /// <inheritdoc/>
    public override IRawCommand WithTake(int value) => new RawCommand(this) { Take = value };

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool SupportsNativePaging
    {
        get => throw null;
    }

    /// <inheritdoc/>
    public override bool IsValid
    {
        get => throw null;
    }

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo()
    {
        throw null;
    }

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool iterable)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IRawCommand Clear()
    {
        throw null;
    }
    IExecutableCommand IExecutableCommand.Clear() => Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(Func<dynamic, object> spec, params object?[]? range)
    {
        throw null;
    }
}