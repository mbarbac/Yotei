namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents an exit selection.
/// </summary>
public class MenuExit : MenuItem
{
    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Exit.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
        => throw new UnExpectedException("Exit actions are not expected to execute.");
}