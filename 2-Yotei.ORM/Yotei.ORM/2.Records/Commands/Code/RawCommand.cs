namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable]
[InheritWiths]
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
    public virtual ICommandExecutor GetExecutor()
    {
        var temp = Connection.Records.CreateCommandExecutor(this);
        return temp;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IRawCommand.Append(Func{dynamic, object}, object?[]?)"/>
    public virtual RawCommand Append(Func<dynamic, object> spec, params object?[]? args)
    {
        throw null;
    }
    IRawCommand IRawCommand.Append(
        Func<dynamic, object> spec, params object?[]? args) => Append(spec, args);

    /// <inheritdoc cref="ICommand.Clear"/>
    public override RawCommand Clear()
    {
        throw null;
    }
    IRawCommand IRawCommand.Clear() => Clear();
}