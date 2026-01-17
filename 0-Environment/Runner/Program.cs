using Color = System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// This executable program.
/// </summary>
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Trace.Listeners.EnsureConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        var str = "Hello world!";
        Console.Write(Color.Green, "Text: ");
        Console.WriteLine(str);

        Console.Write(Color.Green, "Edit: ");
        var r = Console.EditLine(Color.White, Color.Blue, str);
        Console.WriteLine(r);
    }
}
