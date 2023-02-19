namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents an entry in a console menu.
/// </summary>
public class MenuEntry
{
    /// <summary>
    /// Initializes an empty instance.
    /// </summary>
    public MenuEntry() { }

    /// <summary>
    /// Initializes a new instance with the overriden print delegate.
    /// </summary>
    /// <param name="print"></param>
    public MenuEntry(Action print) => Print = print;

    /// <summary>
    /// Initializes a new instance with the overriden print and execute delegates.
    /// </summary>
    /// <param name="print"></param>
    /// <param name="execute"></param>
    public MenuEntry(Action print, Action execute) : this(print) => Execute = execute;

    /// <summary>
    /// Writes in the console the description of this entry, which must end with a new line.
    /// </summary>
    public Action Print
    {
        get => _Print ?? OnPrint;
        init => _Print = value;
    }
    readonly Action? _Print = null;

    /// <summary>
    /// If not prevented by a constructor delegate, writes in the console the description of
    /// this entry, which must end with a new line.
    /// </summary>
    public virtual void OnPrint() => WriteLine();

    /// <summary>
    /// The action associated with this entry.
    /// </summary>
    public Action Execute
    {
        get => _Execute ?? OnExecute;
        init => _Execute = value;
    }
    readonly Action? _Execute = null;

    /// <summary>
    /// If not prevented by a constructor delegate, the action associted with this entry.
    /// </summary>
    public virtual void OnExecute() { }
}