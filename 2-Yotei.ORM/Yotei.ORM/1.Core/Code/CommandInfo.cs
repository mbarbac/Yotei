namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source) : this(source.GetCommandInfo()) { }

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) : this(source.Engine) => Items.Add(source);

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) : this(source.Engine) => Items.Add(source);

    /// <summary>
    /// Initializes a new instance with the given text and a collection of parameters obtained
    /// from the given range of values, if any.
    /// <br/>- If both text and values are used, then those values must be encoded in the text
    /// using bracket specifications, either positional '{n}' or named '{name}' ones, where
    /// the name may or may not start with the engine's parameter prefix. Unused values or
    /// dangling specifications are not allowed.
    /// <br/>- If 'text' is null, then the range of values is just captured without trying to
    /// match their names with any bracket specification. Similarly, if 'range' is empty, then
    /// the text is just captured without intercepting dangling specifications.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string? text, params object?[]? range) : this(engine)
    {
        Items.Add(text, range);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo.IBuilder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string Text => _Text ??= Items.Text;
    string? _Text = null;

    /// <inheritdoc/>
    public IParameterList Parameters => _Parameters ??= Items.Parameters;
    IParameterList? _Parameters = null;

    /// <inheritdoc/>
    public bool IsEmpty => Items.IsEmpty;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommand source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommandInfo source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommandInfo.IBuilder source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ICommandInfo Add(string? text, params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.Add(text, range);
        return done ? builder.CreateInstance() : this;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceText(string? text)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceValues(params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValues(range);
        return done ? builder.CreateInstance() : this;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}