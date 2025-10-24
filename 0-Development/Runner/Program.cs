using static System.ConsoleColor;

namespace Runner;

internal class Program
{
    [SuppressMessage("", "IDE0060")]
    static void Main(string[] args)
    {
        Console.Write("Value: ");
        var value = Console.EditLine(Yellow, Blue, TimeSpan.FromSeconds(5));
        Console.WriteLine(value);
        Console.ReadLine();
    }
}
