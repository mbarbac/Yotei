using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Menu entry for managing artifacts.
/// </summary>
public class MenuBuilder : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Build NuGet Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
    }
}