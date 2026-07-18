namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable(ReturnType = typeof(ICommandInfo))]
public partial class CommandInfo : ICommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public CommandInfo(IConnection connection) => throw null;

    /// <summary>
    /// Initializes a new instance with the contents of the given source, using its default
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source) => throw null;

    /// <summary>
    /// Initializes a new instance with the contents of the given source, using the requested
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source, bool iterable) => throw null;

    /// <summary>
    /// Initializes a new instance with the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => throw null;

    /// <summary>
    /// Initializes a new instance with the given text and the parameters obtained from the given
    /// range of values, if any.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="text"></param>
    public CommandInfo(string text, params object?[]? range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected CommandInfo(CommandInfo other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get => throw null; }

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
    public virtual bool Add(ICommand source) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    public virtual bool Add(ICommand source, bool iterable) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual bool Add(ICommandInfo source) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool Add(string text, params object?[]? range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual bool AddText(string text) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool AddValues(params object?[]? range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual bool ReplaceText(string text) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool ReplaceValues(params object?[]? range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual bool Clear() => throw null;
}