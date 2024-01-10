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
    /// <returns></returns>
    public override string ToString()
    {
        var str = GetText(false, out var parameters);
        if (parameters.Count != 0) str += $"; -- [{string.Join(", ", parameters)}]";
        return str;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; } = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public abstract string GetText(out IParameterList parameters);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public abstract string GetText(bool iterable, out IParameterList parameters);
}