namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    protected virtual ICommandInfoBuilder Builder { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Builder = new CommandInfoBuilder(engine);

    /// <summary>
    /// Initializes a new instance with the given text and optional values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(
        IEngine engine, string? text, params object?[] range)
        => Builder = new CommandInfoBuilder(engine, text, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => Builder = new CommandInfoBuilder(source);

    /// <inheritdoc/>
    public override string ToString() => Builder.ToString()!;

    /// <inheritdoc/>
    public virtual ICommandInfoBuilder ToBuilder() => Builder.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Text => _Text ??= Builder.Text;
    string? _Text;

    /// <inheritdoc/>
    public IParameterList Parameters => _Parameters ??= Builder.Parameters;
    IParameterList? _Parameters;

    /// <inheritdoc/>
    public bool IsEmpty => Builder.IsEmpty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo ReplaceText(string? text)
    {
        var clone = Clone();
        var done = clone.Builder.ReplaceText(text);
        return done ? clone : this;
    }

    /// <inheritdoc/>
    public ICommandInfo ReplaceParameters(IEnumerable<IParameter> range)
    {
        var clone = Clone();
        var done = clone.Builder.ReplaceParameters(range);
        return done ? clone : this;
    }

    /// <inheritdoc/>
    public ICommandInfo ReplaceValues(params object?[] range)
    {
        var clone = Clone();
        var done = clone.Builder.ReplaceValues(range);
        return done ? clone : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfo source)
    {
        var clone = Clone();
        var done = clone.Builder.Add(source);
        return done ? clone : this;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfoBuilder source)
    {
        var clone = Clone();
        var done = clone.Builder.Add(source);
        return done ? clone : this;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(string? text, params object?[] range)
    {
        var clone = Clone();
        var done = clone.Builder.Add(text, range);
        return done ? clone : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Clear()
    {
        var clone = Clone();
        var done = clone.Builder.Clear();
        return done ? clone : this;
    }
}