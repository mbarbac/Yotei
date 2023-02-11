namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a separator in a menu.
/// </summary>
public class RunnerSeparator : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine();

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
        => throw new UnExpectedException("Separator actions are not expected to run.");
}