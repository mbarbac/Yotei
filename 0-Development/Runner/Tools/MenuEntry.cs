namespace Runner;

// ========================================================
/// <summary>
/// Represents an entry in a console menu.
/// </summary>
/// <param name="header"></param>
/// <param name="execute"></param>
public class MenuEntry(string? header = null, Action? execute = null)
{
    readonly string? _Header = header;
    readonly Action? _Execute = execute;

    /// <summary>
    /// Returns the description header of this entry.
    /// </summary>
    /// <returns></returns>
    public virtual string Header() => _Header ?? string.Empty;

    /// <summary>
    /// Invoked to execute the action associated with this entry.
    /// </summary>
    public virtual void Execute() => _Execute?.Invoke();
}