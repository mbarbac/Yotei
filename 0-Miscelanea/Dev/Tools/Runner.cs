using System.Threading.Tasks.Dataflow;

namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents an action to execute.
/// </summary>
public abstract class Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public abstract void PrintHead();

    /// <summary>
    /// Executes this action.
    /// </summary>
    public abstract void Execute();
}