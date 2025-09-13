namespace Yotei.ORM.Records.Code;
/*
// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable<IRawCommand>]
[InheritWiths<IRawCommand>]
public partial class RawCommand : Command, IRawCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <exception cref="System.NullReferenceException"></exception>
    public RawCommand(IConnection connection) : base(connection) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool IsValid => throw null;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo() => throw null;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool iterable) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetEnumerator()
        => Connection.Records.CreateCommandEnumerator(this);

    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default)
        => Connection.Records.CreateCommandEnumerator(this, token);

    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token) => GetAsyncEnumerator(token);

    /// <inheritdoc/>
    public virtual ICommandExecutor GetExecutor()
        => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool SupportsNativePaging => throw null;

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
    public override IRawCommand Clear() => throw null;
    IEnumerableCommand IEnumerableCommand.Clear() => Clear();
    IExecutableCommand IExecutableCommand.Clear() => Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(string text, params object?[]? range) => throw null;

    /// <inheritdoc/>
    public virtual IRawCommand Append(Func<dynamic, object> spec, params object?[]? range) => throw null;
}*/