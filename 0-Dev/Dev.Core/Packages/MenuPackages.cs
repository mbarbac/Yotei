namespace Dev.Packages;

// ========================================================
/// <summary>
/// Manages generated packages.
/// </summary>
public class MenuPackages : MenuItem
{
    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Manages packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        WriteLine(Color.Green, "Managing packages...");
    }
}