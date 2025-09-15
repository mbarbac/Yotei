namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
public partial class CommandInfo : ICommandInfo
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    public CommandInfo(ICommand source, bool iterable) => Items = new(source, iterable);

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => Items = new(source);

    /// <summary>
    /// Initializes a new instance with the contents of the given source
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => Items = new(source);

    /// <summary>
    /// Initializes a new instance with the given text and with the collection of parameters
    /// obtained from the given range of values.
    /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
    /// positional specification or a '{name}' named one. In the later case, 'name' may or may not
    /// start with the engine's prefix.
    /// <br/> Unused values or dangling specifications are not allowed.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string text, params object?[]? range)
        => Items = new(engine, text, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => Items = new(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public virtual ICommandInfo Clone() => new CommandInfo(this);

    // ------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    /// Caching value...
    public string Text => _Text ??= Items.Text;
    string? _Text = null;

    /// <inheritdoc/>
    /// Caching value...
    public IParameterList Parameters => _Parameters ??= Items.Parameters;
    IParameterList? _Parameters = null;

    /// <inheritdoc/>
    /// Using cached values...
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    /// <inheritdoc/>
    public bool IsConsistent() => Items.IsConsistent();

    /// <inheritdoc/>
    public virtual ICommandInfo.IBuilder CreateBuilder() => Items.Clone();

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo Add(ICommand source, bool iterable)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source, iterable);
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
    public virtual ICommandInfo Add(string text, params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.Add(text, range);
        return done ? builder.CreateInstance() : this;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceText(string text)
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