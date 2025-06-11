namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommand"/>
[Cloneable]
[InheritWiths]
public abstract partial class Command : ICommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public Command(IConnection connection) => Connection = connection;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Command(Command source) : this(source.Connection)
    {
        source.ThrowWhenNull();
        Locale = source.Locale;
    }

    /// <inheritdoc/>
    public override string ToString() => GetCommandInfo().ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection
    {
        get => _Connection;
        init => _Connection = value.ThrowWhenNull();
    }
    IConnection _Connection = default!;

    /// <inheritdoc/>
    public Locale Locale
    {
        get => _Locale;
        init => _Locale = value.ThrowWhenNull();
    }
    Locale _Locale = new();

    /// <inheritdoc/>
    public abstract CommandInfo GetCommandInfo();
    ICommandInfo ICommand.GetCommandInfo() => GetCommandInfo();

    /// <inheritdoc/>
    public abstract CommandInfo GetCommandInfo(bool iterable);
    ICommandInfo ICommand.GetCommandInfo(bool iterable) => GetCommandInfo(iterable);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract Command Clear();
    ICommand ICommand.Clear() => Clear();
}