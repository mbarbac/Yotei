namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable<ICommandInfo>]
public partial class CommandInfo : ICommandInfo
{
    protected virtual Builder Items { get; }

    Builder Validate(Builder builder) => builder.IsConsistent
        ? builder
        : throw new ArgumentException("Builder is not consistent.").WithData(builder);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public CommandInfo(IConnection connection) => Items = Validate(new(connection));

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    public CommandInfo(ICommand source, bool iterable) => Items = Validate(new(source, iterable));

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => Items = Validate(new(source));

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => Items = Validate(new(source));

    /// <summary>
    /// Initializes a new instance with the given text and the collection of parameters
    /// obtained from the given range of values, if any.
    /// <br/> If both text and values are used, then the later shall be encoded in the text using
    /// either a positional '{n}' or a named '{name}' specification (where 'name' may or may not
    /// start with the engine prefix).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(
        IConnection connection, string? text, params object?[]? range)
        => Items = Validate(new(connection, text, range));

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => Items = Validate(new(source));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder CreateBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection => Items.Connection;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text => _Text ??= Items.Text;
    string? _Text;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int TextLen => Items.TextLen;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters => _Parameters ??= Items.Parameters;
    IParameterList? _Parameters;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty => Items.IsEmpty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsConsistent => Items.IsConsistent;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommand source, bool iterable)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source, iterable);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommandInfo source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommandInfo.IBuilder source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(string? text, params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.Add(text, range);
        return done ? builder.CreateInstance() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceText(string? text)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceValues(params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValues(range);
        return done ? builder.CreateInstance() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}