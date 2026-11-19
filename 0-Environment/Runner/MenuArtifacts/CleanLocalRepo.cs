using static System.ConsoleColor;
namespace Runner;

// ========================================================
public class CleanLocalRepo : ConsoleMenu.Entry
{
    static string? Repo;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Headline() => "Clean Local NuGet Repo";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, Program.FatSeparator);
        Console.WriteLineEx(true, Green, Headline());
        Console.WriteLineEx(true);

        Repo ??= Program.LocalRepoPath;

        Console.WriteLine(Green, "(Leave blank to abort)");
        Repo = Program.EditDirectory(Repo, "Repo: ");
        if (Repo is null) return;

        Console.WriteLineEx(true);
        Console.WriteLineEx(true, Green, "Deleting:");

        var dir = new DirectoryInfo(Repo);
        var files = dir.GetFiles();

        Console.PreventBufferOverflow(files.Length + 5);
        var row = Console.CursorTop;
        var len = 0;
        var num = 0;

        static void ConsolePosition(int top, int left)
        {
            Console.CursorTop = top;
            Console.CursorLeft = left;
        }

        foreach (var file in files)
        {
            ConsolePosition(row, 0); Console.Write(new string(' ', len + 1));
            ConsolePosition(row, 0); Console.Write(file.FullName);

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