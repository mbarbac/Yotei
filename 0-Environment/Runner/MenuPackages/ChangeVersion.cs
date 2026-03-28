using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class ChangeVersion(Project project) : ConsoleMenuEntry
{
    readonly Project Project = project.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => $"Change Version";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute() { }
}