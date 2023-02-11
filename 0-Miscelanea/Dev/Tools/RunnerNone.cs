namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a none-action to execute, typically used to exit a menu.
/// </summary>
public class RunnerNone : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("None.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
        => throw new UnExpectedException("None actions are not expected to run.");
}