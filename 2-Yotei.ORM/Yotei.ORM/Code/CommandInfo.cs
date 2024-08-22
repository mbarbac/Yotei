namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the given text and optional parameters.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string? text, params object?[] range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source) => throw null;

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Parameters.Engine;

    /// <inheritdoc/>
    public string Text => throw null;

    /// <inheritdoc/>
    public IParameterList Parameters => throw null;

    /// <inheritdoc/>
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    // ----------------------------------------------------
}