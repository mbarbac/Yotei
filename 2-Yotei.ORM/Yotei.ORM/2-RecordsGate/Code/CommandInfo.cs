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
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents of the given source, using its default
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents of the given source, using the requested
    /// iterable mode.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommand source, bool iterable) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo source) => throw null;

    /// <summary>
    /// Initializes a new instance using the contents of the given source.
    /// </summary>
    /// <param name="source"></param>
    public CommandInfo(ICommandInfo.IBuilder source) => throw null;

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder ToBuilder() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty => throw null;
}