namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    /// <inheritdoc/>
    protected virtual Builder Items { get; }

    /// <summary>
    /// Invoked to create the initial repository of contents of this instance.
    /// </summary>
    protected virtual Builder OnInitialize(IEngine engine) => new(engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Items = OnInitialize(engine);

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) : this(source.Engine) => Items.Add(source);

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) : this(source.Engine) => Items.Add(source);

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
    public CommandInfo(
        IEngine engine, string? text, params object?[]? range) : this(engine) => Items.Add(text, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ------------------------------------------------

    /// <inheritdoc cref="ICommandInfo.CreateBuilder"/>
    public virtual Builder CreateBuilder() => Items.Clone();
    ICommandInfo.IBuilder ICommandInfo.CreateBuilder() => CreateBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string Text => Items.Text;

    /// <inheritdoc/>
    public IParameterList Parameters => Items.Parameters;

    /// <inheritdoc/>
    public bool IsEmpty => Items.IsEmpty;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual CommandInfo Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.Clear() => Clear();

    /// <inheritdoc/>
    public virtual CommandInfo ReplaceText(string? text)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.ReplaceText(string? text) => ReplaceText(text);

    /// <inheritdoc/>
    public virtual CommandInfo ReplaceValues(params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValues(range);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.ReplaceValues(params object?[]? range) => ReplaceValues(range);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual CommandInfo Add(ICommandInfo source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.Add(ICommandInfo source) => Add(source);

    /// <inheritdoc/>
    public virtual CommandInfo Add(ICommandInfo.IBuilder source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.Add(ICommandInfo.IBuilder source) => Add(source);

    /// <inheritdoc/>
    public virtual CommandInfo Add(string? text, params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.Add(text, range);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.Add(string? text, params object?[]? range) => Add(text, range);
}