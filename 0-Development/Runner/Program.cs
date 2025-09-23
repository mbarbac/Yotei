namespace Runner;

// ========================================================
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Ambient.AddNewConsoleListener();

        DebugEx.Write(true, ConsoleColor.Green, "Enter value: ");
        var str = ConsoleEx.ReadLine(true);
        ConsoleEx.Write(true, ConsoleColor.Green, "You entered: ");
        ConsoleEx.WriteLine(true, str);

        Thread.Sleep(500);
        var info = ConsoleEx.ReadKey(true, true, TimeSpan.FromSeconds(2));

        //var ch = info is null
        //    ? "[null]"
        //    : info.Value.KeyChar < 32 ? $"[{info.Value.Key}]" : $"{info.Value.KeyChar}";        
        //Console.WriteLine($"Pressed: {ch} ...");

        Console.ReadLine();
    }
}
