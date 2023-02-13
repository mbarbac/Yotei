namespace Dev;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    /// <summary>
    /// Program's entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Menu.Run(
            new Tester.MenuTester(),
            new Builder.MenuBuilder(),
            new Janitor.MenuJanitor());
    }
}
