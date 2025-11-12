namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable<ICommandInfo>]
public partial class CommandInfo : ICommandInfo
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public CommandInfo(IConnection connection) => Items = new(connection);

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents from the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder CreateBuilder() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int TextLen { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsConsistent { get => throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommand source, bool iterable) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommandInfo source) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommandInfo.IBuilder source) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(string text, params object?[]? range) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceText(string text, bool strict = true) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceParameters(params object?[]? range) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo Clear() => throw null;
}