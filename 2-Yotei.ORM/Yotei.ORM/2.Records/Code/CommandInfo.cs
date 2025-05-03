namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
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
        var builder = new Builder(source);

        Text = builder.Text;
        Parameters = builder.Parameters;
    }

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source)
    {
        var builder = new Builder(source);

        Text = builder.Text;
        Parameters = builder.Parameters;
    }

    /// <summary>
    /// Initializes a new instance with the given text and parameters obtained from the given
    /// range of values.
    /// <br/> If text is null, then the range of values is just captured without any attempt
    /// of matching their names with any text specifications. Conversely, if no elements are
    /// given in the range, then the text is just captured without intercepting any dangling
    /// specifications.
    /// <br/> Parameter specifications in the text are always bracket ones, and can either be
    /// positional '{n}' or named '{name}' ones. Positional one refer to the ordinal of the
    /// element in the range of values. Named ones contain the name of the parameter or the
    /// unique property of the given anonymous item, and 'name' may or may not start with
    /// the engine parameters' prefix.
    /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
    /// <br/> You can use raw (non-bracketed) parameter names as you wish.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string? text, params object?[]? range)
    {
        var builder = new Builder(engine, text, range);

        Text = builder.Text;
        Parameters = builder.Parameters;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source)
    {
        var builder = new Builder(source);

        Text = builder.Text;
        Parameters = builder.Parameters;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Parameters.Count == 0) return Text;

        var pars = $"[{string.Join(", ", Parameters)}]";
        return Text.Length == 0 ? pars : $"{Text} : {pars}";
    }

    /// <inheritdoc/>
    public virtual ICommandInfo.IBuilder GetBuilder() => new Builder(this);

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
    public ICommandInfo Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo ReplaceText(string? text)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ICommandInfo ReplaceParameters(IEnumerable<IParameter> range)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceParameters(range);
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