using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class MenuTester(bool all) : ConsoleMenu.Entry
{
    /// <summary>
    /// Determines if all tests are to be executed, instead of trying to filter them.
    /// </summary>
    public bool All { get; } = all;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => All
        ? "Execute All Tests"
        : "Execute Filtered Tests";
}