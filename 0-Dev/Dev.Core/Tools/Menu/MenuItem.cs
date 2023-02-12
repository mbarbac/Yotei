namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a possible selection in a menu.
/// </summary>
public abstract class MenuItem
{
    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public abstract void PrintHead();

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public abstract void Execute();
}