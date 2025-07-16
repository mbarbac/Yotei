namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable]
[InheritWiths]
public partial class RawCommand : EnumerableCommand, IRawCommand
{
    FragmentRaw.Master _RawFragment;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection) => _RawFragment = new(this);

    /// <summary>
    /// Initializes a new instance using the given dynamic lambda expression and the optional
    /// collection of arguments.
    /// <br/> If an optional collection of arguments is provided, then the specification must
    /// resolve to a string, and those arguments must be encoded in that string using either a
    /// positional '{n}' or named '{name}' specification. If it is provided but the expression
    /// does not resolve to a string, or if there is a mismatch between the encoded arguments
    /// and the actual ones in that collection, then an exception is thrown.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    /// <param name="args"></param>
    public RawCommand(IConnection connection, Func<dynamic, object> spec, params object?[]? args)
        : this(connection)
        => Append(spec, args);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source) => _RawFragment = source._RawFragment.Clone();

    /// <inheritdoc/>
    public virtual ICommandExecutor GetExecutor()
    {
        var temp = Connection.Records.CreateCommandExecutor(this);
        return temp;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool IsValid => _RawFragment.Count > 0;

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo() => (CommandInfo)_RawFragment.Visit().CreateInstance();

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo(bool _) => GetCommandInfo();

    // ----------------------------------------------------

    /// <inheritdoc cref="IRawCommand.Append(Func{dynamic, object}, object?[]?)"/>
    public virtual RawCommand Append(Func<dynamic, object> spec, params object?[]? args)
    {
        _RawFragment.Capture(spec, args);
        return this;
    }
    IRawCommand IRawCommand.Append(
        Func<dynamic, object> spec, params object?[]? args) => Append(spec, args);

    /// <inheritdoc cref="ICommand.Clear"/>
    public override RawCommand Clear()
    {
        _RawFragment.Clear();
        base.Clear();

        return this;
    }
    IRawCommand IRawCommand.Clear() => Clear();
    IExecutableCommand IExecutableCommand.Clear() => Clear();
}