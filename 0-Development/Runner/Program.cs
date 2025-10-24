using static System.ConsoleColor;

namespace Runner;

internal class Program
{
    [SuppressMessage("", "IDE0060")]
    static void Main(string[] args)
    {
        Ambient.GetOrAddConsoleListener();
        Console.Write("Value: ");
        var value = Console.EditLine(true, Yellow, Blue, TimeSpan.FromSeconds(5), "Hello world!");
        Console.WriteLine(value);
        Console.ReadLine();
    }
}
