namespace Dev.Builder;

// ========================================================
/// <summary>
/// Builds and updates packages.
/// </summary>
public class RunBuilder : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Manage packages.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        Program.MenuRun(
            new Tools.RunnerSeparator(),
            new LocalPushAll(),
            new LocalPushSelect(),
            new Tools.RunnerSeparator(),
            new LocalIncreaseAll(),
            new LocalIncreaseSelect());
    }
}