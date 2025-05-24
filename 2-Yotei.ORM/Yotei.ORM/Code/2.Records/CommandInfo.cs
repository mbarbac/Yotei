namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public sealed partial class CommandInfo : ICommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine)
    {
        Text = string.Empty;
        Parameters = new ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source)
    {
        source.ThrowWhenNull();

        Text = source.Text;
        Parameters = new ParameterList(source.Engine, source.Parameters);
    }

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source)
    {
        source.ThrowWhenNull();

        Text = source.Text;
        Parameters = new ParameterList(source.Engine, source.Parameters);
    }

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
    public CommandInfo(IEngine engine, string? text, params object?[]? range)
    {
        var builder = new Builder(engine, text, range);

        Text = builder.Text;
        Parameters = new ParameterList(engine, builder.Parameters);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    CommandInfo(CommandInfo source)
    {
        source.ThrowWhenNull();

        Text = source.Text;
        Parameters = new ParameterList(source.Engine, source.Parameters);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Parameters.Count == 0) return Text;

        var pars = $"[{string.Join(", ", Parameters)}]";
        return Text.Length == 0 ? pars : $"{Text} : {pars}";
    }

    /// <inheritdoc/>
    public ICommandInfo.IBuilder GetBuilder() => new Builder(this);

    // ------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Parameters.Engine;

    /// <inheritdoc/>
    public string Text { get; }

    /// <inheritdoc/>
    public IParameterList Parameters { get; }

    /// <inheritdoc/>
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    // ------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo ReplaceText(string? text)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo ReplaceValues(params object?[]? range)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceValues(range);
        return done ? builder.ToInstance() : this;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfo source)
    {
        var builder = GetBuilder();
        var done = builder.Add(source);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfo.IBuilder source)
    {
        var builder = GetBuilder();
        var done = builder.Add(source);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(string? text, params object?[]? range)
    {
        var builder = GetBuilder();
        var done = builder.Add(text, range);
        return done ? builder.ToInstance() : this;
    }
}