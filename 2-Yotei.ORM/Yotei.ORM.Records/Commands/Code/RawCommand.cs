namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable]
[WithGenerator]
public partial class RawCommand : Command, IRawCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection)
        => _Info = new CommandInfo(connection.Engine);

    /// <summary>
    /// Initializes a new instance with the given text and optional arguments.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If the optional list of arguments is used, then their associated names or positions
    /// must be referenced in the given text, and all arguments must be used. Each can be:
    /// <br/> - a raw value, that appears in the text agains a '{n}' positional specification. A
    /// new parameter is added using a name generated automatically.
    /// <br/> - a regular parameter, that appears in the text either by its '{name}' or by its
    /// '{n}' positional specification. If that name is a duplicated one, then an exception is
    /// thrown.
    /// <br/> - an anonymous type, that appears in the text either by the '{name}' of its unique
    /// property, or by its '{n}' positional specification. If that name does not start by the
    /// engine's parameter prefix, then it is added automatically.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    public RawCommand(IConnection connection, string? specs, params object?[] args)
        : this(connection)
        => Append(specs, args);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    /// <!--We use the base constructor as we don't need anything else here.-->
    protected RawCommand(RawCommand source) : base(source.Connection) => _Info = source._Info;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandEnumerator GetEnumerator() => Connection.Records.CreateCommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default) => Connection.Records.CreateCommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(CancellationToken token) => GetAsyncEnumerator(token);

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    ICommandInfo _Info;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo() => _Info;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool iterable) => _Info;

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <!-- We don't know what king of engine we are dealing with, so this default implementation
    /// must instruct the enumerator to emulate.-->
    public virtual bool NativePaging => false;

    /// <inheritdoc/>
    public int Skip
    {
        get => _Skip;
        set => _Skip = value >= 0 ? value : -1;
    }
    int _Skip = -1;

    /// <inheritdoc/>
    public int Take
    {
        get => _Take;
        set => _Take = value >= 0 ? value : -1;
    }
    int _Take = -1;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRawCommand Append(string? specs, params object?[] args)
    {
        _Info = _Info.Add(specs, args);
        return this;
    }

    /// <inheritdoc/>
    public IRawCommand Clear()
    {
        _Info = _Info.Clear();
        return this;
    }
}