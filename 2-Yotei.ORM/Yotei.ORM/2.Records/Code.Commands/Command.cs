namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommand"/>
[Cloneable]
[WithGenerator]
public abstract partial class Command : ICommand
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Command(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();
        Locale = new Locale();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var info = GetCommandInfo(iterable: false);
        var text = info.Text;

        if (info.Parameters.Count != 0) text += $"; [{string.Join(", ", info.Parameters)}]";
        return text;
    }

    /// <inheritdoc/>
    public IConnection Connection { get; }

    /// <inheritdoc/>
    public Locale Locale
    {
        get => _Locale ??= new Locale();
        set => _Locale = value.ThrowWhenNull();
    }
    Locale? _Locale;

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo(bool iterable);
}