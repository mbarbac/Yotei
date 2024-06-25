namespace Runner;

// ========================================================
/// <summary>
/// Represents an entry in a console menu.
/// </summary>
public class MenuEntry(string? header = null, Action? execute = null)
{
    readonly string? _Header = header;
    readonly Action? _Execute = execute;

    /// <summary>
    /// Obtains an appropriate description of this entry.
    /// </summary>
    /// <returns></returns>
    public virtual string Header() => _Header ?? string.Empty;

    /// <summary>
    /// Invoked to execute the action associated with this entry.
    /// </summary>
    public virtual void Execute() => _Execute?.Invoke();
}