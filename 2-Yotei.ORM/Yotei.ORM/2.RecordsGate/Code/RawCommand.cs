namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable<IRawCommand>]
[InheritWiths<IRawCommand>]
public partial class RawCommand : EnumerableCommand, IRawCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source) => throw null;

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool SupportsNativePaging => throw null;

    /// <inheritdoc/>
    public override bool IsValid => throw null;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo() => throw null;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool iterable) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IRawCommand Clear() => throw null;
    IExecutableCommand IExecutableCommand.Clear() => Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(string text, params object?[]? range) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(Func<dynamic, object> spec, params object?[]? range) => throw null;
}