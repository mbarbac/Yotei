using static System.ConsoleColor;

namespace Runner;

// ========================================================
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "IDE0210")]
    static void Main()
    {
        Console.WriteLine("Hello, World!");        
        var str = Console.EditLine(null, White, Green);
        Console.WriteLine(str);
        Console.ReadKey();
    }
}
