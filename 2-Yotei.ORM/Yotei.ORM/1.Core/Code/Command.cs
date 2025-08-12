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
    protected Command(Command source)
    {
        source.ThrowWhenNull();

        Connection = source.Connection;
        Locale = source.Locale;
    }

    /// <inheritdoc/>
    public override string ToString() => GetCommandInfo().ToString() ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection
    {
        get => _Connection;
        init => _Connection = value.ThrowWhenNull();
    }
    IConnection _Connection = default!;

    protected IEngine Engine => Connection.Engine;

    /// <inheritdoc/>
    public Locale Locale
    {
        get => _Locale;
        init => _Locale = value.ThrowWhenNull();
    }
    Locale _Locale = new();

    /// <inheritdoc/>
    public abstract bool IsValid { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract ICommand Clear();
}