namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an entry in a console menu.
/// </summary>
/// <param name="header"></param>
/// <param name="execute"></param>
[DebuggerDisplay("{Header()}")]
public class ConsoleMenuEntry(string? header = null, Action? execute = null)
{
    readonly string? _Header = header;
    readonly Action? _Execute = execute;

    /// <summary>
    /// Gets the description of this entry (to be used in the menu).
    /// </summary>
    /// <returns></returns>
    public virtual string Header() => _Header ?? string.Empty;

    /// <summary>
    /// The action invoked when this entry is selected.
    /// </summary>
    public virtual void Execute() => _Execute?.Invoke();
}