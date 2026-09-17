namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommand"/>
/// </summary>
[Cloneable(ReturnType = typeof(ICommand))]
public abstract partial class Command : ICommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Command(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Command(Command other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Connection = other.Connection;
        Locale = other.Locale;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Locale Locale
    {
        get => field ??= new Locale();
        set => field = value.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ICommandInfo GetCommandInfo();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    public abstract ICommandInfo GetCommandInfo(bool iterable);
}