using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class MenuTester : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Execute Tests";

    /// <inheritdoc/>
    public override void Execute() { }
}