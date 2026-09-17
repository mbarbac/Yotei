namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable(ReturnType = typeof(ICommandInfo))]
public partial class CommandInfo : ICommandInfo
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => Items = new(source);

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => Items = new(source);

    /// <summary>
    /// Intializes a new instance with the given text and the collection of parameters obtained
    /// from the given optional values have been added to it.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If text is not null, then the values must be encoded using either a positional
    /// '{n}' specification, or a named '{name}' one (where if 'name' is not prefixed with the
    /// engine's prefix, it is added automatically).
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="values"></param>
    public CommandInfo(
        IEngine engine, string? text, params object?[]? values) => Items = new(engine, text, values);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected CommandInfo(CommandInfo other) => Items = new(other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text => field ??= Items.Text;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters => field ??= Items.Parameters;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty => Items.IsEmpty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsConsistent => Items.IsConsistent;

    /// <summary>
    /// </summary>
    public virtual bool IsValid => Items.IsValid;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo Add(ICommandInfo source)
    {
        var builder = ToBuilder();
        var done = builder.Add(source);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo Add(ICommandInfo.IBuilder source)
    {
        var builder = ToBuilder();
        var done = builder.Add(source);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="values"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo Add(string? text, params object?[]? values)
    {
        var builder = ToBuilder();
        var done = builder.Add(text, values);
        return done ? builder.ToInstance() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo ReplaceText(string? text)
    {
        var builder = ToBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo ReplaceValues(params object?[]? values)
    {
        var builder = ToBuilder();
        var done = builder.ReplaceValues(values);
        return done ? builder.ToInstance() : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual ICommandInfo Clear()
    {
        var builder = ToBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}