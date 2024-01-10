#pragma warning disable IDE0044 // Make field readonly

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRawCommand"/>
/// </summary>
[Cloneable]
public partial class RawCommand : ORM.Code.Command, IRawCommand
{
    readonly Internal.CommandInfo Info;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection)
        : base(connection)
        => Info = new Internal.CommandInfo(connection.Engine);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source)
        : base(source.Connection)
        => Info = new Internal.CommandInfo(source.Info.CommandText, source.Info.Parameters);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICommandEnumerator GetEnumerator()
        => Connection.Records.CreateCommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default)
        => Connection.Records.CreateCommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token)
        => GetAsyncEnumerator(token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override string GetText(out IParameterList parameters)
    {
        parameters = Info.Parameters;
        return Info.CommandText;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override string GetText(
        bool iterable, out IParameterList parameters) => GetText(out parameters);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IRawCommand Append(string? specs, params object?[] args)
    {
        Info.Add(specs, args);
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public IRawCommand Append(Func<dynamic, object> specs) => throw null;
}