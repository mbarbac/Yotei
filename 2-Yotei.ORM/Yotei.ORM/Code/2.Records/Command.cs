namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommand"/>
[Cloneable]
[InheritWiths]
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
    /// <param name="source"></param>
    protected Command(Command source)
    {
        Connection = source.ThrowWhenNull().Connection;
        Locale = source.Locale;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection { get; }

    /// <inheritdoc/>
    public Locale Locale
    {
        get => _Locale;
        init => _Locale = value.ThrowWhenNull();
    }
    Locale _Locale = new();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract ICommand Clear();
}