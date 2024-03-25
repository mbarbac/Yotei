namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommand"/>
[Cloneable]
public abstract partial class Command(IConnection connection) : ICommand
{
    public override string ToString()
    {
        var info = GetCommandInfo(iterable: false);
        var text = info.Text;

        if (info.Parameters.Count != 0) text += $"; [{string.Join(", ", info.Parameters)}]";
        return text;
    }

    /// <inheritdoc/>
    public IConnection Connection { get; } = connection.ThrowWhenNull();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo();

    /// <inheritdoc/>
    public abstract ICommandInfo GetCommandInfo(bool iterable);
}