namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    protected virtual Builder Items { get; }
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
    public CommandInfo(ICommand source) : this(source.GetCommandInfo()) { }

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
    /// refering to the parameter position in the given collection, or named '{name}' ones,
    /// where name contains the name of the parameter or the name of the unique property of
    /// an anonymous item. In both cases, 'name' may or may not start with the prefix given
    /// by the engine.
    /// <br/>- No unused parameters are allowed, neither dangling bracket specifications.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(
        IEngine engine, string? text, params object?[]? range) : this(engine)
        => Items.Add(text, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) : this(new Builder(source)) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Parameters.Count == 0) return Text;

        var pars = $"[{string.Join(", ", Parameters)}]";
        return Text.Length == 0 ? pars : $"{Text} : {pars}";
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => new(this);
    ICommandInfo.IBuilder ICommandInfo.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Parameters.Engine;

    /// <inheritdoc/>
    public string Text => _Text ??= Items.Text;
    string? _Text = null;

    /// <inheritdoc/>
    public IParameterList Parameters => _Parameters ??= Items.Parameters;
    IParameterList? _Parameters = null;

    /// <inheritdoc/>
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(ICommandInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        var sensitive = Engine.CaseSensitiveNames;
        if (string.Compare(Text, other.Text, !sensitive) != 0) return false;

        return Parameters.Equals(other.Parameters);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ICommandInfo);

    public static bool operator ==(CommandInfo? host, ICommandInfo? other) => host?.Equals(other) ?? false;

    public static bool operator !=(CommandInfo? host, ICommandInfo? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = Text.GetHashCode();
        code = HashCode.Combine(code, Parameters);
        return code;
    }

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
    public virtual ICommandInfo ReplaceText(string? text)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceText(text);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.ReplaceText(string? text) => ReplaceText(text);

    /// <inheritdoc/>
    public virtual ICommandInfo ReplaceValues(params object?[]? range)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValues(range);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.ReplaceValues(params object?[]? range) => ReplaceValues(range);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual CommandInfo Add(ICommand source)
    {
        var builder = CreateBuilder();
        var done = builder.Add(source);
        return done ? builder.CreateInstance() : this;
    }
    ICommandInfo ICommandInfo.Add(ICommand source) => Add(source);

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