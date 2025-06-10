namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => throw null;

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => throw null;

    /// <summary>
    /// Initializes a new instance using the given text and the collection of parameters
    /// obtained from the given range of values, if any.
    /// <br/>- If <paramref name="text"/> is null, then the range of values is just captured
    /// without any attempts of matching their names with bracket specifications. Similarly,
    /// if <paramref name="range"/> is empty, then the text is captured without intercepting
    /// any dangling specifications.
    /// <br/>- Parameter specifications must always be bracket ones, either positional '{n}'
    /// or named '{name}' ones, where name contains the name of the parameter or the name of
    /// the unique property of an anonymous item. In both cases, 'name' may or may not start
    /// with the engine parameters' prefix.
    /// <br/>- No unused parameters are allowed, neither dangling bracket specifications.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string? text, params object?[]? range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => throw null;

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ------------------------------------------------

    /// <inheritdoc cref="ICommandInfo.CreateBuilder"/>
    public virtual Builder CreateBuilder() => throw null;
    ICommandInfo.IBuilder ICommandInfo.CreateBuilder() => CreateBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string Text { get; }

    /// <inheritdoc/>
    public IParameterList Parameters { get; }

    /// <inheritdoc/>
    public bool IsEmpty { get; }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo Clear() => throw null;

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceText(string? text) => throw null;

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceValues(params object?[]? range) => throw null;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommandInfo source) => throw null;

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommandInfo.IBuilder source) => throw null;

    /// <inheritdoc/>
    public virtual ICommandInfo Add(string? text, params object?[]? range) => throw null;
}