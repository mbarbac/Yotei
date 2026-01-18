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

        Console.WriteLine();
        Console.Write("Edit: ");
        var span = TimeSpan.FromSeconds(5);
        var str = Console.EditLineEx(true, span, Color.Yellow, Color.Blue, "Hello world!");
        Console.WriteLine($"Text: {str ?? "<null>"}");

        //var menu = new ConsoleMenu { ToDebug = true }
        //.Add(new("One"))
        //.Add(new("Two"));

        //var target = menu with { Timeout = TimeSpan.FromSeconds(10) };
        //Debug.Assert(target.Count == 2);
        //Debug.Assert(target[0].Header() == "One");
        //Debug.Assert(target[1].Header() == "Two");
        //Debug.Assert(target.ToDebug = true);
        //Debug.Assert(target.Timeout.Seconds == 10);
    }
}
