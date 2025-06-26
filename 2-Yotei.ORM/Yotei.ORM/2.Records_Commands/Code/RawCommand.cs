namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
[Cloneable]
[InheritWiths]
public partial class RawCommand : EnumerableCommand, IRawCommand
{
    readonly CommandInfo.Builder Info;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(
        IConnection connection) : base(connection) => Info = new CommandInfo.Builder(Engine);

    /// <summary>
    /// Initializes a new instance using the given text and optional arguments.
    /// <br/>- If the text is null, the it is ignored and the optional arguments are captured
    /// without any attempts of matching their names with any text specification.
    /// <br/>- Similarly, if there are no elements in the optional list of arguments, the text
    /// is captured without intercepting any dangling spcifications.
    /// <br/>- Otherwise, specifications are always bracket ones, either positional '{n}' ones,
    /// of named '{name}' ones (where name may or may not start with the engine parameters'
    /// prefix). No unused elements in the optional list of arguments are allowed, neither
    /// dangling specifications in the given text.
    /// <br/>- If text is not null, then a space is added if needed.
    /// <br/>- Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public RawCommand(
        IConnection connection,
        string? text, params object?[] args) : this(connection) => Append(text, args);

    /// <summary>
    /// Initializes a new instance using the text and parameters obtained from parsing the given
    /// dynamic lambda expression.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    public RawCommand(
        IConnection connection,
        Func<dynamic, object> spec) : this(connection) => Append(spec);

    /// <summary>
    /// Initializes a new instance using the text obtained from the what the given dynamic lambda
    /// expression reduces to.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    public RawCommand(
        IConnection connection,
        Func<dynamic, string> spec) : this(connection) => Append(spec);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source) => Info = source.Info.Clone();

    /// <inheritdoc/>
    public override string ToString() => Info.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo() => Info.CreateInstance();

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo(bool _) => Info.CreateInstance();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual RawCommand Append(string? text, params object?[]? args)
    {
        Info.Add(text, args);
        return this;
    }
    IRawCommand IRawCommand.Append(string? text, params object?[]? args) => Append(text, args);

    /// <inheritdoc/>
    public virtual RawCommand Append<T>(Func<dynamic, T> spec)
    {
        spec.ThrowWhenNull();

        var visitor = Connection.Records.CreateDbTokenVisitor(Locale);
        var info = visitor.Visit(spec);

        Info.Add(info);
        return this;
    }
    IRawCommand IRawCommand.Append<T>(Func<dynamic, T> spec) => Append(spec);

    /// <inheritdoc/>
    public override RawCommand Clear()
    {
        Info.Clear();

        base.Clear();
        return this;
    }
    IRawCommand IRawCommand.Clear() => Clear();
}