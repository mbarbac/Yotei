namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public partial class CommandInfo : ICommandInfo
{
    /// <inheritdoc/>
    public Builder GetBuilder() => throw null;
    ICommandInfo.IBuilder ICommandInfo.GetBuilder() => GetBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string Text { get; }

    /// <inheritdoc/>
    public IParameterList Parameters { get; }

    /// <inheritdoc/>
    public bool IsEmpty { get; }

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual CommandInfo ReplaceText(string? text);

    /// <inheritdoc/>
    public virtual CommandInfo ReplaceValues(params object?[]? range);

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual CommandInfo Clear();

    /// <inheritdoc/>
    public virtual CommandInfo Add(ICommandInfo source);

    /// <inheritdoc/>
    public virtual CommandInfo Add(IBuilder source);

    /// <inheritdoc/>
    public virtual CommandInfo Add(string? text, params object?[]? range);
}