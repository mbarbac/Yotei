namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a console menu item.
/// </summary>
public class MenuItem
{
    Action PrintFunc = () => WriteLine("Empty");
    Action ExecuteFunc = () => { };

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public MenuItem() { }

    /// <summary>
    /// Initializes a new instance with the given print and execute actions.
    /// </summary>
    /// <param name="print"></param>
    /// <param name="execute"></param>
    public MenuItem(Action print, Action execute)
    {
        PrintFunc = print.ThrowIfNull();
        ExecuteFunc = execute.ThrowIfNull();
    }

    /// <summary>
    /// Writes in the console the description of this item, which must end with a new line.
    /// </summary>
    public virtual void Print() => PrintFunc();

    /// <summary>
    /// Executes the actions associated with this item.
    /// </summary>
    public virtual void Execute() => ExecuteFunc();
}