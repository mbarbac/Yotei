namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable]
[InheritWiths]
public partial class RawCommand : Command, IRawCommand
{
    readonly ICommandInfo.IBuilder _Info;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection)
        : base(connection)
        => _Info = new CommandInfo.Builder(connection.Engine);

    /// <summary>
    /// Initializes a new instance using the given text and optional arguments.
    /// <br/> If the text is null, the it is ignored and the optional arguments are captured
    /// without any attempts of matching their names with any text specification.
    /// <br/> Similarly, if there are no elements in the optional list of arguments, the text
    /// is captured without intercepting any dangling spcifications.
    /// <br/> Otherwise, specifications are always bracket ones, either positional '{n}' ones,
    /// of named '{name}' ones (where name may or may not start with the engine parameters'
    /// prefix). No unused elements in the optional list of arguments are allowed, neither
    /// dangling specifications in the given text.
    /// <br/> If text is not null, then a space is added if needed.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public RawCommand(
        IConnection connection,
        string? text, params object?[] args) : this(connection) => Append(text, args);

    /// <summary>
    /// Initializes a new instance using the text and arguments obtained from parsing the given
    /// dynamic lambda expression.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    public RawCommand(
        IConnection connection,
        Func<dynamic, object> spec) : this(connection) => Append(spec);
    
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source)
        : base(source)
        => _Info = source._Info.Clone();

    /// <inheritdoc/>
    public override string ToString() => _Info.ToString()!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>We cannot determine if the arbitrary contents are such that support native
    /// paging, even if the underlying engine does so. Hence, the default value of this property
    /// if <c>false</c>, but it can be set to <c>true</c> under the caller's responsability.
    /// <br/> Note that the setter DOES NOT generate a new instance.</remarks>
    public virtual bool SupportsNativePaging
    {
        get => _SupportsNativePaging && Connection.Engine.SupportsNativePaging;
        set => _SupportsNativePaging = value;
    }
    bool _SupportsNativePaging = false;

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

    /// <inheritdoc/>
    public ICommandEnumerator GetEnumerator() => Connection.Records.CreateCommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default) => Connection.Records.CreateCommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo() => _Info.ToInstance();

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool _) => _Info.ToInstance();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(string? text, params object?[]? args)
    {
        _Info.Add(text, args);
        return this;
    }

    /// <inheritdoc/>
    public virtual IRawCommand Append(Func<dynamic, object> spec)
    {
        spec.ThrowWhenNull();

        var visitor = Connection.Records.CreateDbTokenVisitor();
        var info = visitor.Visit(spec);

        _Info.Add(info);
        return this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IRawCommand Clear()
    {
        _Info.Clear();
        return this;
    }
}