namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRawCommand"/>
/// </summary>
[Cloneable(ReturnType = typeof(IRawCommand))]
public partial class RawCommand : EnumerableCommand, IRawCommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected RawCommand(RawCommand other) : base(other) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandExecutor GetExecutor() => Connection.Records.CreateExecutor(this);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsValid => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override ICommandInfo GetCommandInfo() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    public override ICommandInfo GetCommandInfo(bool iterable) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public virtual IRawCommand Append(ICommandInfo info) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual IRawCommand Append(string text, params object?[]? args) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public virtual IRawCommand Append(Func<dynamic, object> spec) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool SupportsNativePaging => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override IRawCommand Skip(int value)
    {
        base.Skip(value);
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override IRawCommand Take(int value)
    {
        base.Take(value);
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IRawCommand Clear()
    {
        base.Clear();
        return this;
    }

    IExecutableCommand IExecutableCommand.Clear() => Clear();
}