namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommand"/>
/// </summary>
[Cloneable<ICommand>]
[InheritWiths<ICommand>]
public abstract partial class Command : ICommand
{
    /// <summary>
    /// Initializes a new empty instance.
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
        RawLocale = source.RawLocale;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => GetCommandInfo().ToString()!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ILocale Locale => RawLocale ?? Connection.Locale;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ILocale? RawLocale { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsValid { get; }

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ICommand Clear();
}