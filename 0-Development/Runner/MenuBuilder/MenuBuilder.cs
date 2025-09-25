using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuBuilder : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Build Packages";

    /// <inheritdoc/>
    public override void Execute() { }
}