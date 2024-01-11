namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// Represents a core text-based records-oriented command.
/// </summary>
[Cloneable]
public sealed partial class CoreCommand : ORM.Code.Command, IEnumerableCommand, IExecutableCommand
{
    readonly Internal.CommandInfo Info;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public CoreCommand(IConnection connection) : base(connection)
    {
        Info = new Internal.CommandInfo(connection.Engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="text"></param>
    public CoreCommand(IConnection connection, string text) : this(connection)
    {
        Info.Add(text);
    }

    /// <summary>
    /// Initializes a new instance with the given parameter.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="parameter"></param>
    public CoreCommand(IConnection connection, IParameter parameter) : this(connection)
    {
        Info.Add(parameter);
    }

    /// <summary>
    /// Initializes a new instance with the parameters from the given range.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="range"></param>
    public CoreCommand(IConnection connection, IEnumerable<IParameter> range) : this(connection)
    {
        Info.Add(range);
    }

    /// <summary>
    /// Initializes a new instance with the given text and arguments.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public CoreCommand(IConnection connection, string text, params object?[] args) : this(connection)
    {
        Info.Add(text, args);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    CoreCommand(CoreCommand source) : base(source.Connection)
    {
        Info = new Internal.CommandInfo(source.Info.CommandText, source.Info.Parameters);
    }

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
    public override string GetText(bool iterable, out IParameterList parameters)
    {
        return GetText(out parameters);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to the contents of this instance the given text and optional arguments.
    /// <br/> Text can be null if not used.
    /// <br/> Arguments, if used, shall be encoded in the given text using:
    /// <br/> - The '{name}' of a regular parameter included in the optional array.
    /// <br/> - The '{name}' of the unique property of an anonymous type included in the optional
    /// array, as in 'new { name = value }'.
    /// <br/> - A '{n}' positional specification, where 'n' is the index in the optional array
    /// from where to obtain the value.
    /// <br/>
    /// <br/> Returns this instance to be used in a fluent syntax fashion.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public CoreCommand Add(string? text, params object?[] args)
    {
        Info.Add(text, args);
        return this;
    }
}