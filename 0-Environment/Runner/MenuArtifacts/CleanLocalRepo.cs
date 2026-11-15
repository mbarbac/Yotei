using System.Security.Cryptography;
using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class CleanLocalRepo : ConsoleMenu.Entry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Clean Local Repo";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.SlimSeparator);
        Console.WriteLineEx(true, Green, "Clean local NuGet repo");
        Console.WriteLineEx(true);

        Console.WriteLine(Green, "(Leave path blank to abort)");
        var path = Program.EditDirectory(Program.LocalRepoPath, "Path: ");
        if (path is null) return;

        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, "Deleting:");

        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles();

        Console.PreventBufferOverflow(files.Length + 5);
        var row = Console.CursorTop;
        var len = 0;
        var num = 0;

        foreach (var file in files)
        {
            Console.CursorTop = row;
            Console.CursorLeft = 0; Console.Write(new string(' ', len + 1));
            Console.CursorLeft = 0; Console.Write(file.FullName);

            try { file.Delete(); len = file.FullName.Length; num++; }
            catch (Exception e)
            {
                Debug.Write($"File: {file.FullName}");
                Console.WriteLineEx(true, Red, $"Error: {e.Message}");
                row = Console.CursorTop;
                len = 0;
            }
        }

        Console.CursorTop = row;
        Console.CursorLeft = 0; Console.Write(new string(' ', len + 1));
        Console.CursorLeft = 0;
        
        Console.WriteEx(true, Green, "Total deleted: ");
        Console.WriteLineEx(true, num.ToString());
    }
}