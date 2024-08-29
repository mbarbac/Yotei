namespace Yotei.ORM.Records.Code;

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
    public Command(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();
        Locale = new(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// The locale used of the culture-sensitive objected in the database.
    /// <br/> Its default value is the one of the thread that created this instance.
    /// </summary>
    public Locale Locale { get; }

    /// <summary>
    /// Obtains the information needed to run this command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    public abstract ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to run this command, using the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    public abstract ICommandInfo GetCommandInfo(bool iterable);
}