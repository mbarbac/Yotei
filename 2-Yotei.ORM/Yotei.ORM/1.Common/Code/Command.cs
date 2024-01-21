namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommand"/>
/// </summary>
/// <param name="connection"></param>
[Cloneable]
public abstract partial class Command(IConnection connection) : ICommand
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString()
    {
        var info = GetCommandInfo(iterable: false);
        var text = info.CommandText;

        if (info.Parameters.Count != 0) text += $"; [{string.Join(", ", info.Parameters)}]";
        return text;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; } = connection.ThrowWhenNull();

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