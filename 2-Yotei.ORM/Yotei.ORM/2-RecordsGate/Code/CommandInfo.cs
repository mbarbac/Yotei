namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable(ReturnType = typeof(ICommandInfo))]
public partial class CommandInfo : ICommandInfo
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance using the contents of the given source, using its default
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source) => Items = new(source);

    /// <summary>
    /// Initializes a new instance using the contents of the given source, using the requested
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source, bool iterable) => Items = new(source, iterable);

    /// <summary>
    /// Initializes a new instance using the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => Items = new(source);

    /// <summary>
    /// Initializes a new instance using the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!source.IsConsistent) throw new InvalidOperationException(
            "The given builder is in an inconsistent state.")
            .WithData(source);

        Items = new(source);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected CommandInfo(CommandInfo other) => Items = new(other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
    public string Text { get => field ??= Items.Text; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get => field ??= Items.Parameters; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty => Items.IsEmpty;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommand source) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(string text, params object?[]? values) => throw null;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual ICommandInfo AddText(string text) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual ICommandInfo AddValues(params object?[]? values) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceText(string text) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual ICommandInfo ReplaceValues(params object?[]? values) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo Clear() => throw null;
}