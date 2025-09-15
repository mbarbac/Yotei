namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
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